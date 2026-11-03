using System;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class RiderController : Controller
    {
        private readonly RiderRepository _riderRepo = new RiderRepository();
        private readonly RestaurantRepository _restRepo = new RestaurantRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();

        private bool IsRiderAuthenticated()
        {
            return Session["RiderId"] != null && Session["UserRole"]?.ToString() == "Rider";
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (IsRiderAuthenticated())
            {
                return RedirectToAction("Dashboard");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Please enter Rider email/username and password.");
                return View(model);
            }

            var rider = _riderRepo.Login(model.Email.Trim().ToLower(), model.Password);
            if (rider != null)
            {
                Session["RiderId"] = rider.RiderId;
                Session["RiderName"] = rider.RiderName;
                Session["RiderEmail"] = rider.Email;
                Session["UserRole"] = "Rider";

                FormsAuthentication.SetAuthCookie(rider.Email, false);
                TempData["Success"] = "Welcome, Delivery Partner " + rider.RiderName + "!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid rider email/username or password.");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "Delivery partner logged out.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            if (!IsRiderAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int riderId = Convert.ToInt32(Session["RiderId"]);
            var rider = _riderRepo.GetById(riderId);
            var myDeliveries = _riderRepo.GetMyDeliveries(riderId);
            var availableDeliveries = _riderRepo.GetAvailableDeliveries();
            var affiliations = _riderRepo.GetRiderAffiliations(riderId);
            var allRestaurants = _restRepo.GetAllVerified();

            ViewBag.Rider = rider;
            ViewBag.AvailableDeliveries = availableDeliveries;
            ViewBag.Affiliations = affiliations;
            ViewBag.AllRestaurants = allRestaurants;

            return View(myDeliveries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetStatus(string status)
        {
            if (!IsRiderAuthenticated()) return RedirectToAction("Login");

            int riderId = Convert.ToInt32(Session["RiderId"]);
            var rider = _riderRepo.GetById(riderId);
            if (rider != null)
            {
                rider.CurrentStatus = status ?? "Available";
                string msg;
                _riderRepo.Update(rider, out msg);
                TempData["Success"] = $"Your live status updated to: {rider.CurrentStatus}";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyAffiliation(int restaurantId)
        {
            if (!IsRiderAuthenticated()) return RedirectToAction("Login");

            int riderId = Convert.ToInt32(Session["RiderId"]);
            string message;
            if (_riderRepo.AddAffiliation(riderId, restaurantId, out message))
            {
                _notifRepo.AddNotification("Restaurant", restaurantId, "New Rider Affiliation Request",
                    $"{Session["RiderName"]} requested to partner with your restaurant. Pending verification.", "Rider", "/Restaurant/Dashboard");
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptDelivery(int orderId)
        {
            if (!IsRiderAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int riderId = Convert.ToInt32(Session["RiderId"]);
            string riderName = Session["RiderName"]?.ToString() ?? "Rider";
            if (_riderRepo.AcceptDelivery(orderId, riderId))
            {
                var order = _orderRepo.GetById(orderId);
                int? custId = order != null ? (int?)order.CustomerId : null;
                string orderNum = order != null && !string.IsNullOrEmpty(order.OrderNumber) ? order.OrderNumber : ("#FD" + (1000 + orderId));
                
                _notifRepo.AddNotification("Customer", custId, "Rider On the Way!", riderName + " picked up your Order " + orderNum + " and is heading to your address.", "Order", "/Order/History");
                _notifRepo.AddNotification("Admin", null, "Delivery Dispatched", "Order " + orderNum + " is out for delivery with " + riderName + ".", "Order", "/Admin/Orders");
                TempData["Success"] = "You accepted delivery for Order " + orderNum + ". Drive safe!";
            }
            else
            {
                TempData["Error"] = "Could not assign this delivery.";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteDelivery(int orderId)
        {
            if (!IsRiderAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int riderId = Convert.ToInt32(Session["RiderId"]);
            if (_riderRepo.CompleteDelivery(orderId, riderId))
            {
                var order = _orderRepo.GetById(orderId);
                int? custId = order != null ? (int?)order.CustomerId : null;
                string orderNum = order != null && !string.IsNullOrEmpty(order.OrderNumber) ? order.OrderNumber : ("#FD" + (1000 + orderId));

                _notifRepo.AddNotification("Customer", custId, "Order Delivered!", "Your Order " + orderNum + " has been delivered. Enjoy your delicious food!", "Order", "/Order/History");
                _notifRepo.AddNotification("Admin", null, "Order Completed", "Order " + orderNum + " delivered successfully.", "Order", "/Admin/Orders?tab=accepted");
                TempData["Success"] = "Order " + orderNum + " marked as DELIVERED. Great job!";
            }
            else
            {
                TempData["Error"] = "Could not update delivery status.";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public new ActionResult Profile()
        {
            if (!IsRiderAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int riderId = Convert.ToInt32(Session["RiderId"]);
            var rider = _riderRepo.GetById(riderId);
            if (rider == null)
            {
                return RedirectToAction("Logout");
            }

            return View(rider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Rider model)
        {
            if (!IsRiderAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int riderId = Convert.ToInt32(Session["RiderId"]);
            model.RiderId = riderId;

            string message;
            if (_riderRepo.Update(model, out message))
            {
                Session["RiderName"] = model.RiderName;
                TempData["Success"] = "Rider profile updated successfully.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }
    }
}
