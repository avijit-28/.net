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
        private readonly RestaurantRepository _restRepo = new RestaurantRepository();
        private readonly RiderRepository _riderRepo = new RiderRepository();
        private readonly StaffRepository _staffRepo = new StaffRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        private bool IsAdminAuthenticated()
        {
            return Session["AdminId"] != null && Session["UserRole"]?.ToString() == "Admin";
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
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Please enter Admin username/email and password.");
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

            ModelState.AddModelError("", "Invalid admin credentials. Please use avi@123 / 28072003.");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "Admin logged out successfully.";
            return RedirectToAction("Login");
        }

        // ==========================
        // 1. DASHBOARD OVERVIEW
        // ==========================
        [HttpGet]
        public ActionResult Dashboard()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            // Auto-reject pending orders older than 5 minutes
            int autoRejected = _orderRepo.AutoRejectOrders();
            if (autoRejected > 0)
            {
                ViewBag.AutoRejectedNotice = $"{autoRejected} pending orders older than 5 minutes were auto-rejected.";
            }

            var summary = _adminRepo.GetDashboardSummary();

            // Populate multi-role approval lists & data
            summary.PendingRestaurants = _restRepo.GetPendingVerification();
            summary.PendingStaff = _staffRepo.GetAllPendingStaff();
            summary.PendingRiders = _riderRepo.GetRestaurantRiders(null, true);
            summary.PendingDishes = _staffRepo.GetPendingDishes(null);
            summary.RecentTransactions = _adminRepo.GetAllTransactions().Take(10).ToList();
            summary.ActiveBulletins = _adminRepo.GetBulletins();

            var recentPending = _orderRepo.GetPendingOrders();
            var recentAccepted = _orderRepo.GetAcceptedOrders();
            var recentRejected = _orderRepo.GetRejectedOrders();

            summary.RecentOrders = recentPending.Concat(recentAccepted).Concat(recentRejected)
                .OrderByDescending(x => x.OrderedAt)
                .Take(10)
                .ToList();

            return View(summary);
        }

        // ==========================
        // 2. APPROVALS & VERIFICATIONS
        // ==========================
        [HttpGet]
        public ActionResult Approvals(string tab = "restaurants")
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            ViewBag.ActiveTab = (tab ?? "restaurants").ToLower();
            var summary = new AdminDashboardViewModel
            {
                PendingRestaurants = _restRepo.GetPendingVerification(),
                PendingStaff = _staffRepo.GetAllPendingStaff(),
                PendingRiders = _riderRepo.GetRestaurantRiders(null, true),
                PendingDishes = _staffRepo.GetPendingDishes(null)
            };

            return View(summary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveRestaurant(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_restRepo.VerifyRestaurant(id, adminId, out message))
            {
                _notifRepo.AddNotification("Restaurant", id, "Restaurant Approved!", "Your restaurant has been verified and is now live on FoodReady.", "Approval", "/Restaurant/Dashboard");
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "restaurants" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectRestaurant(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_restRepo.RejectRestaurant(id, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "restaurants" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveStaff(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_staffRepo.VerifyStaff(id, "Admin", adminId, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "staff" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectStaff(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_staffRepo.RejectStaff(id, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "staff" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveRiderAffiliation(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_riderRepo.VerifyAffiliation(id, "Admin", adminId, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "riders" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectRiderAffiliation(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_riderRepo.RejectAffiliation(id, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "riders" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveDish(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_staffRepo.ApproveDish(id, "Admin", adminId, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "dishes" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectDish(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_staffRepo.RejectDish(id, out message))
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Approvals", new { tab = "dishes" });
        }

        // ==========================
        // 3. RIDER FLEET TRACKING
        // ==========================
        [HttpGet]
        public ActionResult Riders()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var allRiders = _riderRepo.GetAll();
            var affiliations = _riderRepo.GetRestaurantRiders(null, false);
            ViewBag.Affiliations = affiliations;

            return View(allRiders);
        }

        // ==========================
        // 4. TRANSACTIONS & PAYMENTS
        // ==========================
        [HttpGet]
        public ActionResult Transactions()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var transactions = _adminRepo.GetAllTransactions();
            return View(transactions);
        }

        // ==========================
        // 5. PUBLIC BULLETINS & ANNOUNCEMENTS
        // ==========================
        [HttpGet]
        public ActionResult Bulletins()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var bulletins = _adminRepo.GetBulletins();
            return View(bulletins);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBulletin(Bulletin bulletin)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            bulletin.CreatedByAdminId = Convert.ToInt32(Session["AdminId"]);
            if (_adminRepo.AddBulletin(bulletin))
            {
                _notifRepo.AddNotification("All", null, "New Announcement: " + bulletin.Title, bulletin.Message, "Info", "/Home/Index");
                TempData["Success"] = "Public announcement broadcasted successfully.";
            }
            else
            {
                TempData["Error"] = "Unable to create announcement.";
            }

            return RedirectToAction("Bulletins");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBulletin(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            if (_adminRepo.DeleteBulletin(id))
            {
                TempData["Success"] = "Bulletin announcement archived.";
            }
            else
            {
                TempData["Error"] = "Unable to remove bulletin.";
            }

            return RedirectToAction("Bulletins");
        }

        // ==========================
        // 6. ORDER MANAGEMENT & AUTO-REJECT
        // ==========================
        [HttpGet]
        public ActionResult Orders(string tab = "pending")
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            // Auto-reject pending orders older than 5 minutes
            int autoRejected = _orderRepo.AutoRejectOrders();
            if (autoRejected > 0)
            {
                ViewBag.AutoRejectedNotice = $"{autoRejected} pending order(s) older than 5 minutes were automatically rejected by system policy.";
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
        public ActionResult TriggerAutoReject()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int count = _orderRepo.AutoRejectOrders();
            TempData["Success"] = $"Auto-reject execution complete. {count} expired order(s) (> 5 mins) were rejected.";
            return RedirectToAction("Orders", new { tab = "pending" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptOrder(int orderId)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_orderRepo.AcceptOrder(orderId, adminId, out message))
            {
                var order = _orderRepo.GetDetails(orderId);
                int? custId = order != null ? (int?)order.CustomerId : null;
                _notifRepo.AddNotification("Customer", custId, "Order Accepted!", "Your Order #FD" + (1000 + orderId) + " was accepted and sent to kitchen.", "Order", "/Order/History");
                _notifRepo.AddNotification("Restaurant", null, "Order Confirmed by Admin", "Order #FD" + (1000 + orderId) + " confirmed. Start preparation.", "Order", "/Restaurant/Dashboard");
                _notifRepo.AddNotification("Rider", null, "Upcoming Delivery", "Order #FD" + (1000 + orderId) + " accepted. Ready for pickup shortly.", "Info", "/Rider/Dashboard");

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
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            int adminId = Convert.ToInt32(Session["AdminId"]);
            string message;
            if (_orderRepo.RejectOrder(orderId, adminId, reason, out message))
            {
                var order = _orderRepo.GetDetails(orderId);
                int? custId = order != null ? (int?)order.CustomerId : null;
                _notifRepo.AddNotification("Customer", custId, "Order Rejected", "Order #FD" + (1000 + orderId) + " was rejected: " + (reason ?? "Unable to fulfill at this time."), "Alert", "/Order/History");

                TempData["Success"] = "Order #FD" + (1000 + orderId) + " rejected.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Orders", new { tab = "pending" });
        }

        // ==========================
        // 7. FOOD ITEMS CRUD & AVAILABILITY
        // ==========================
        [HttpGet]
        public ActionResult FoodItems()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var items = _foodRepo.GetAll();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFoodItem(FoodItem item, HttpPostedFileBase imageFile)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

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

            item.IsVerified = true;
            item.VerifiedByRole = "Admin";
            item.VerifiedBy = Convert.ToInt32(Session["AdminId"]);

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
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

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
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

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
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

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
