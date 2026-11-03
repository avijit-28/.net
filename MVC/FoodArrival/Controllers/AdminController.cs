using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminRepository _adminRepo = new AdminRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        private bool IsAdminAuthenticated()
        {
            return Session["AdminId"] != null;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (IsAdminAuthenticated())
            {
                return RedirectToAction("Dashboard");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = _adminRepo.Login(model.Email.Trim().ToLower(), model.Password);
            if (admin != null)
            {
                Session["AdminId"] = admin.AdminId;
                Session["AdminName"] = admin.AdminName;
                Session["AdminEmail"] = admin.Email;
                Session["UserRole"] = "Admin";

                FormsAuthentication.SetAuthCookie(admin.Email, false);
                TempData["Success"] = "Welcome, Admin " + admin.AdminName + "!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid admin email or password.");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "Admin logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            // Auto-reject pending orders older than 5 minutes
            _orderRepo.AutoRejectOrders();

            var summary = _adminRepo.GetDashboardSummary();
            var recentPending = _orderRepo.GetPendingOrders();
            var recentAccepted = _orderRepo.GetAcceptedOrders();
            var recentRejected = _orderRepo.GetRejectedOrders();

            // Recent orders list for preview
            var allRecent = recentPending.Concat(recentAccepted).Concat(recentRejected)
                .OrderByDescending(x => x.OrderedAt)
                .Take(10)
                .ToList();

            summary.RecentOrders = allRecent;

            return View(summary);
        }

        [HttpGet]
        public ActionResult Orders(string tab = "pending")
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            // Auto-reject pending orders older than 5 minutes
            int autoRejected = _orderRepo.AutoRejectOrders();
            if (autoRejected > 0)
            {
                ViewBag.AutoRejectedNotice = $"{autoRejected} pending order(s) older than 5 minutes were automatically rejected.";
            }

            var pendingOrders = _orderRepo.GetPendingOrders();
            var acceptedOrders = _orderRepo.GetAcceptedOrders();
            var rejectedOrders = _orderRepo.GetRejectedOrders();

            ViewBag.PendingCount = pendingOrders.Count;
            ViewBag.AcceptedCount = acceptedOrders.Count;
            ViewBag.RejectedCount = rejectedOrders.Count;

            tab = (tab ?? "pending").ToLower();
            ViewBag.ActiveTab = tab;

            List<Order> ordersToDisplay;
            if (tab == "accepted")
            {
                ordersToDisplay = acceptedOrders;
            }
            else if (tab == "rejected")
            {
                ordersToDisplay = rejectedOrders;
            }
            else
            {
                ordersToDisplay = pendingOrders;
            }

            return View(ordersToDisplay);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptOrder(int orderId)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_orderRepo.AcceptOrder(orderId, adminId, out message))
            {
                var order = _orderRepo.GetDetails(orderId);
                int? custId = order != null ? (int?)order.CustomerId : null;
                _notifRepo.AddNotification("Customer", custId, "Order Accepted!", "Your Order #FD" + (1000 + orderId) + " was accepted and sent to kitchen.", "Order", "/Order/History");
                _notifRepo.AddNotification("Restaurant", null, "Order Confirmed by Admin", "Order #FD" + (1000 + orderId) + " confirmed. Start preparation.", "Order", "/Restaurant/Dashboard");
                _notifRepo.AddNotification("Rider", null, "Upcoming Delivery", "Order #FD" + (1000 + orderId) + " accepted. Will be ready for delivery shortly.", "Info", "/Rider/Dashboard");

                TempData["Success"] = "Order #FD" + (1000 + orderId) + " accepted successfully.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Orders", new { tab = "pending" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectOrder(int orderId, string reason)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_orderRepo.RejectOrder(orderId, adminId, reason, out message))
            {
                TempData["Success"] = "Order #FD" + (1000 + orderId) + " rejected.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Orders", new { tab = "pending" });
        }

        [HttpGet]
        public ActionResult FoodItems()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            var items = _foodRepo.GetAll();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFoodItem(FoodItem item, HttpPostedFileBase imageFile)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string uniqueName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileName;
                string dirPath = Server.MapPath("~/Content/images");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                string targetPath = Path.Combine(dirPath, uniqueName);
                imageFile.SaveAs(targetPath);
                item.ImageUrl = "/Content/images/" + uniqueName;
            }

            if (string.IsNullOrWhiteSpace(item.ImageUrl))
            {
                item.ImageUrl = "/Content/images/margherita.jpg";
            }

            string message;
            int newId;
            if (_foodRepo.Insert(item, out message, out newId))
            {
                TempData["Success"] = "Food item '" + item.ItemName + "' added successfully.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("FoodItems");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFoodItem(FoodItem item, HttpPostedFileBase imageFile)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string uniqueName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileName;
                string dirPath = Server.MapPath("~/Content/images");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                string targetPath = Path.Combine(dirPath, uniqueName);
                imageFile.SaveAs(targetPath);
                item.ImageUrl = "/Content/images/" + uniqueName;
            }

            string message;
            if (_foodRepo.Update(item, out message))
            {
                TempData["Success"] = "Food item '" + item.ItemName + "' updated successfully.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("FoodItems");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFoodItem(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            string message;
            if (_foodRepo.Delete(id, out message))
            {
                TempData["Success"] = "Food item marked as inactive/removed.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("FoodItems");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleAvailability(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            var item = _foodRepo.GetById(id);
            if (item != null)
            {
                item.IsAvailable = !item.IsAvailable;
                string message;
                _foodRepo.Update(item, out message);
                TempData["Success"] = "'" + item.ItemName + "' availability toggled to " + (item.IsAvailable ? "Available" : "Unavailable") + ".";
            }

            return RedirectToAction("FoodItems");
        }
    }
}
