using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Attributes;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class HomeController : Controller
    {
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();
        private readonly StaffRepository _staffRepo = new StaffRepository();
        private readonly RiderRepository _riderRepo = new RiderRepository();
        private readonly RestaurantRepository _restRepo = new RestaurantRepository();

        public ActionResult Index()
        {
            var allItems = _foodRepo.GetAvailable();
            ViewBag.FeaturedItems = allItems.Take(8).ToList();
            ViewBag.PizzaItems = allItems.Where(x => x.Category == "Pizza").Take(4).ToList();
            ViewBag.BurgerItems = allItems.Where(x => x.Category == "Burger").Take(4).ToList();
            ViewBag.IndianItems = allItems.Where(x => x.Category == "Indian").Take(4).ToList();
            ViewBag.Categories = new List<string> { "All items", "Pizza", "Burger", "Indian", "Chinese", "Drinks", "Snacks" };

            // Check notifications for current user/role
            string role = Session["UserRole"]?.ToString();
            int? userId = null;
            if (role == "Customer" && Session["CustomerId"] != null) userId = Convert.ToInt32(Session["CustomerId"]);
            else if (role == "Rider" && Session["RiderId"] != null) userId = Convert.ToInt32(Session["RiderId"]);
            else if (role == "Restaurant" && Session["RestaurantId"] != null) userId = Convert.ToInt32(Session["RestaurantId"]);
            else if (role == "RestaurantStaff" && Session["StaffId"] != null) userId = Convert.ToInt32(Session["StaffId"]);
            else if (role == "Admin" && Session["AdminId"] != null) userId = Convert.ToInt32(Session["AdminId"]);

            ViewBag.Notifications = _notifRepo.GetNotifications(role, userId);

            return View(allItems);
        }

        [HttpGet]
        public ActionResult GetNotifications()
        {
            string role = Session["UserRole"]?.ToString();
            int? userId = null;
            if (role == "Customer" && Session["CustomerId"] != null) userId = Convert.ToInt32(Session["CustomerId"]);
            else if (role == "Rider" && Session["RiderId"] != null) userId = Convert.ToInt32(Session["RiderId"]);
            else if (role == "Restaurant" && Session["RestaurantId"] != null) userId = Convert.ToInt32(Session["RestaurantId"]);
            else if (role == "RestaurantStaff" && Session["StaffId"] != null) userId = Convert.ToInt32(Session["StaffId"]);
            else if (role == "Admin" && Session["AdminId"] != null) userId = Convert.ToInt32(Session["AdminId"]);

            var list = _notifRepo.GetNotifications(role, userId);
            int unreadCount = list.Count(x => !x.IsRead);

            var result = list.Select(x => new
            {
                id = x.NotificationId,
                title = x.Title,
                message = x.Message,
                type = x.NotificationType,
                isRead = x.IsRead,
                time = x.CreatedAt.ToString("hh:mm tt"),
                url = x.TargetUrl ?? (role == "Rider" ? "/Rider/Dashboard" : (role == "Restaurant" ? "/Restaurant/Dashboard" : (role == "RestaurantStaff" ? "/Staff/Dashboard" : (role == "Admin" ? "/Admin/Dashboard" : "/Order/History"))))
            });

            return Json(new { success = true, unreadCount = unreadCount, notifications = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MarkNotificationsRead()
        {
            string role = Session["UserRole"]?.ToString();
            int? userId = null;
            if (role == "Customer" && Session["CustomerId"] != null) userId = Convert.ToInt32(Session["CustomerId"]);
            else if (role == "Rider" && Session["RiderId"] != null) userId = Convert.ToInt32(Session["RiderId"]);
            else if (role == "Restaurant" && Session["RestaurantId"] != null) userId = Convert.ToInt32(Session["RestaurantId"]);
            else if (role == "RestaurantStaff" && Session["StaffId"] != null) userId = Convert.ToInt32(Session["StaffId"]);
            else if (role == "Admin" && Session["AdminId"] != null) userId = Convert.ToInt32(Session["AdminId"]);

            _notifRepo.MarkAllAsRead(role, userId);
            return Json(new { success = true });
        }

        public ActionResult About()
        {
            ViewBag.Message = "About FoodReady - Fast, Fresh, Safe Food Delivery.";
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            string role = Session["UserRole"]?.ToString();
            var vm = new ContactViewModel
            {
                SenderRole = !string.IsNullOrEmpty(role) ? role : "Customer"
            };

            if (role == "Customer")
            {
                vm.SenderName = Session["CustomerName"]?.ToString();
                vm.SenderEmail = Session["CustomerEmail"]?.ToString();
                vm.SenderPhone = Session["CustomerPhone"]?.ToString();
            }
            else if (role == "RestaurantStaff")
            {
                vm.SenderName = Session["StaffName"]?.ToString();
                vm.SenderEmail = Session["StaffEmail"]?.ToString();
                int staffId = Convert.ToInt32(Session["StaffId"]);
                var staff = _staffRepo.GetById(staffId);
                if (staff != null)
                {
                    vm.SpecificRestaurantId = staff.RestaurantId;
                    vm.SpecificRestaurantName = staff.RestaurantName;
                    vm.SenderPhone = staff.PhoneNumber;
                    var rest = _restRepo.GetById(staff.RestaurantId);
                    if (rest != null)
                    {
                        vm.SpecificRestaurantPhone = rest.PhoneNumber;
                        vm.SpecificRestaurantAddress = rest.Address;
                    }
                }
            }
            else if (role == "Rider")
            {
                vm.SenderName = Session["RiderName"]?.ToString();
                vm.SenderEmail = Session["RiderEmail"]?.ToString();
                int riderId = Convert.ToInt32(Session["RiderId"]);
                var rider = _riderRepo.GetById(riderId);
                if (rider != null) vm.SenderPhone = rider.PhoneNumber;
                vm.RiderAffiliatedRestaurants = _riderRepo.GetRiderAffiliations(riderId);
            }
            else if (role == "Restaurant")
            {
                vm.SenderName = Session["RestaurantName"]?.ToString();
                vm.SenderEmail = Session["RestaurantEmail"]?.ToString();
                int restId = Convert.ToInt32(Session["RestaurantId"]);
                var rest = _restRepo.GetById(restId);
                if (rest != null)
                {
                    vm.SpecificRestaurantId = rest.RestaurantId;
                    vm.SpecificRestaurantName = rest.RestaurantName;
                    vm.SenderPhone = rest.PhoneNumber;
                    vm.SpecificRestaurantAddress = rest.Address;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateCustomAntiForgeryToken]
        public ActionResult SendMessage(ContactViewModel model)
        {
            string role = Session["UserRole"]?.ToString() ?? model.SenderRole ?? "Customer";
            string subject = string.IsNullOrWhiteSpace(model.Subject) ? "Support Inquiry" : model.Subject.Trim();
            string msg = model.Message?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(msg))
            {
                TempData["Error"] = "Please enter your message details.";
                return RedirectToAction("Contact");
            }

            // 1. If Staff: message routed ONLY to specific restaurant where he works
            if (role == "RestaurantStaff" && Session["StaffId"] != null)
            {
                int staffId = Convert.ToInt32(Session["StaffId"]);
                var staff = _staffRepo.GetById(staffId);
                int restId = staff != null ? staff.RestaurantId : (model.SpecificRestaurantId ?? 1);
                string staffName = staff != null ? staff.StaffName : (model.SenderName ?? "Kitchen Staff");
                string staffCode = staff != null ? staff.StaffCode : "Staff";
                string restName = staff != null ? staff.RestaurantName : "Restaurant Kitchen";

                _notifRepo.AddNotification("Restaurant", restId,
                    $"Staff Query: {subject}",
                    $"Staff {staffName} ({staffCode}) sent message: {msg}",
                    "Staff", "/Restaurant/Dashboard");

                TempData["Success"] = $"Your message has been sent directly to your restaurant employer ({restName}).";
                return RedirectToAction("Contact");
            }

            // 2. If Rider: message routed ONLY to that particular restaurant where the rider is connected
            if (role == "Rider" && Session["RiderId"] != null)
            {
                int riderId = Convert.ToInt32(Session["RiderId"]);
                var rider = _riderRepo.GetById(riderId);
                string riderName = rider != null ? rider.RiderName : (model.SenderName ?? "Delivery Rider");
                string riderPhone = rider != null ? rider.PhoneNumber : model.SenderPhone;

                int targetRestId = model.SpecificRestaurantId ?? 0;
                var riderAffils = _riderRepo.GetRiderAffiliations(riderId);

                // Lock to affiliated restaurant
                if (targetRestId <= 0 && riderAffils.Any())
                {
                    targetRestId = riderAffils.First().RestaurantId;
                }

                if (targetRestId > 0)
                {
                    var matchAffil = riderAffils.FirstOrDefault(a => a.RestaurantId == targetRestId);
                    string targetRestName = matchAffil != null ? matchAffil.RestaurantName : "Partner Kitchen";

                    _notifRepo.AddNotification("Restaurant", targetRestId,
                        $"Rider Query: {subject}",
                        $"Delivery Partner {riderName} ({riderPhone}) sent message: {msg}",
                        "Rider", "/Restaurant/Dashboard");

                    TempData["Success"] = $"Your message has been sent directly to {targetRestName}.";
                }
                else
                {
                    _notifRepo.AddNotification("Admin", null,
                        $"Rider General Query: {subject}",
                        $"Delivery Partner {riderName} ({riderPhone}): {msg}",
                        "Rider", "/Admin/Dashboard");

                    TempData["Success"] = "Your message has been sent to FoodReady Delivery Operations Desk.";
                }
                return RedirectToAction("Contact");
            }

            // 3. If Restaurant Owner: message routed directly to Admin
            if (role == "Restaurant" && Session["RestaurantId"] != null)
            {
                int restId = Convert.ToInt32(Session["RestaurantId"]);
                var rest = _restRepo.GetById(restId);
                string restName = rest != null ? rest.RestaurantName : (model.SenderName ?? "Restaurant Partner");

                _notifRepo.AddNotification("Admin", null,
                    $"Restaurant Partner Query: {subject}",
                    $"{restName} (Owner: {rest?.OwnerName}, Phone: {rest?.PhoneNumber}): {msg}",
                    "Merchant", "/Admin/Approvals");

                TempData["Success"] = "Your message has been sent directly to the FoodReady Admin Operations Desk.";
                return RedirectToAction("Contact");
            }

            // 4. If Customer (or public guest): message routed DIRECTLY to Admin!
            string custName = !string.IsNullOrWhiteSpace(model.SenderName) ? model.SenderName : "Customer";
            string phone = model.SenderPhone ?? "";
            string email = model.SenderEmail ?? "";

            _notifRepo.AddNotification("Admin", null,
                $"Customer Helpline Query: {subject}",
                $"{custName} (Phone: {phone}, Email: {email}): {msg}",
                "Support", "/Admin/Dashboard");

            TempData["Success"] = "Thank you! Your inquiry has been routed directly to the Central Admin Helpline desk.";
            return RedirectToAction("Contact");
        }
    }
}
