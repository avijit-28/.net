using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly RestaurantRepository _restRepo = new RestaurantRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();
        private readonly StaffRepository _staffRepo = new StaffRepository();
        private readonly RiderRepository _riderRepo = new RiderRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();

        private bool IsRestaurantAuthenticated()
        {
            return Session["RestaurantId"] != null && Session["UserRole"]?.ToString() == "Restaurant";
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (IsRestaurantAuthenticated())
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
                ModelState.AddModelError("", "Please enter restaurant email/username and password.");
                return View(model);
            }

            string verificationMsg;
            var rest = _restRepo.Login(model.Email.Trim().ToLower(), model.Password, out verificationMsg);
            if (rest != null)
            {
                Session["RestaurantId"] = rest.RestaurantId;
                Session["RestaurantName"] = rest.RestaurantName;
                Session["RestaurantEmail"] = rest.Email;
                Session["UserRole"] = "Restaurant";

                TempData["Success"] = "Welcome, " + rest.RestaurantName + " Kitchen Partner!";
                return RedirectToAction("Dashboard");
            }

            if (!string.IsNullOrEmpty(verificationMsg))
            {
                ModelState.AddModelError("", verificationMsg);
            }
            else
            {
                ModelState.AddModelError("", "Invalid restaurant credentials or inactive account.");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "Restaurant partner logged out.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard(string section = "orders")
        {
            if (!IsRestaurantAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            var restaurant = _restRepo.GetById(restId);
            var orders = _restRepo.GetOrders(restId);
            var menuItems = _foodRepo.GetByRestaurant(restId);
            var staffList = _staffRepo.GetByRestaurant(restId);
            var proposedDishes = _staffRepo.GetPendingDishes(restId);
            var affiliatedRiders = _riderRepo.GetRestaurantRiders(restId, null);
            var transactions = _restRepo.GetTransactions(restId);

            ViewBag.ActiveSection = section.ToLower();
            ViewBag.Restaurant = restaurant;
            ViewBag.MenuItems = menuItems;
            ViewBag.StaffList = staffList;
            ViewBag.ProposedDishes = proposedDishes;
            ViewBag.AffiliatedRiders = affiliatedRiders;
            ViewBag.Transactions = transactions;

            return View(orders);
        }

        // --- ORDER PROGRESSION WORKFLOW ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            if (!IsRestaurantAuthenticated())
            {
                return RedirectToAction("Login");
            }

            _restRepo.UpdateDeliveryStatus(orderId, status);

            var order = _orderRepo.GetById(orderId);
            int? custId = order != null ? (int?)order.CustomerId : null;
            int? riderId = order != null ? order.RiderId : null;
            string orderNum = order != null && !string.IsNullOrEmpty(order.OrderNumber) ? order.OrderNumber : ("#FD" + (1000 + orderId));

            if (status == "ReadyForPickup")
            {
                if (riderId.HasValue)
                {
                    _notifRepo.AddNotification("Rider", riderId.Value, "Order Ready for Pickup!", "Order " + orderNum + " is prepared and ready for delivery pickup.", "Order", "/Rider/Dashboard");
                }
                if (custId.HasValue)
                {
                    _notifRepo.AddNotification("Customer", custId.Value, "Your Food is Prepared!", "Order " + orderNum + " is hot & packed, waiting for rider pickup.", "Order", "/Order/History");
                }
            }
            else if (status == "Preparing")
            {
                if (custId.HasValue)
                {
                    _notifRepo.AddNotification("Customer", custId.Value, "Kitchen Cooking Your Order", "Chef is currently preparing Order " + orderNum + " with fresh ingredients.", "Order", "/Order/History");
                }
            }

            TempData["Success"] = "Order " + orderNum + " status updated to " + status + ".";
            return RedirectToAction("Dashboard", new { section = "orders" });
        }

        // --- MENU MANAGEMENT ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDish(FoodItem item, HttpPostedFileBase imageFile)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            item.RestaurantId = restId;
            item.IsVerified = true;
            item.VerifiedByRole = "Restaurant";
            item.VerifiedBy = restId;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string uniqueName = "dish_" + Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileName;
                string dirPath = Server.MapPath("~/Content/images");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                string targetPath = Path.Combine(dirPath, uniqueName);
                imageFile.SaveAs(targetPath);
                item.ImageUrl = "/Content/images/" + uniqueName;
            }
            else
            {
                item.ImageUrl = "/Content/images/margherita.jpg";
            }

            string message;
            int newId;
            if (_foodRepo.Insert(item, out message, out newId))
            {
                TempData["Success"] = $"Dish '{item.ItemName}' added to restaurant menu.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "menu" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleDishAvailability(int id)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            var item = _foodRepo.GetById(id);
            if (item != null)
            {
                item.IsAvailable = !item.IsAvailable;
                string msg;
                _foodRepo.Update(item, out msg);
                TempData["Success"] = $"'{item.ItemName}' is now {(item.IsAvailable ? "In Stock" : "Out of Stock")}.";
            }

            return RedirectToAction("Dashboard", new { section = "menu" });
        }

        // --- STAFF PMS & VERIFICATION ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveStaff(int staffId)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            string message;
            if (_staffRepo.VerifyStaff(staffId, "Restaurant", restId, out message))
            {
                TempData["Success"] = "Kitchen staff approved and enabled.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "staff" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectStaff(int staffId)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_staffRepo.RejectStaff(staffId, out message))
            {
                TempData["Success"] = "Staff application rejected.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "staff" });
        }

        // --- STAFF-PROPOSED DISH APPROVAL ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveProposedDish(int foodItemId)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            string message;
            if (_staffRepo.ApproveDish(foodItemId, "Restaurant", restId, out message))
            {
                TempData["Success"] = "Dish approved! It is now live on your active menu.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "proposed" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectProposedDish(int foodItemId)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_staffRepo.RejectDish(foodItemId, out message))
            {
                TempData["Success"] = "Dish proposal rejected.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "proposed" });
        }

        // --- RIDER AFFILIATIONS ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveRider(int affiliationId)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            string message;
            if (_riderRepo.VerifyAffiliation(affiliationId, "Restaurant", restId, out message))
            {
                TempData["Success"] = "Rider verified and linked for deliveries from your kitchen.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "riders" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectRider(int affiliationId)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            string message;
            if (_riderRepo.RejectAffiliation(affiliationId, out message))
            {
                TempData["Success"] = "Rider affiliation request declined.";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard", new { section = "riders" });
        }

        // --- RESTAURANT PROFILE & DETAILS ---
        [HttpGet]
        public ActionResult Details()
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            var rest = _restRepo.GetById(restId);
            return View(rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Restaurant model, HttpPostedFileBase bannerImage)
        {
            if (!IsRestaurantAuthenticated()) return RedirectToAction("Login");

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            model.RestaurantId = restId;

            if (bannerImage != null && bannerImage.ContentLength > 0)
            {
                string fileName = Path.GetFileName(bannerImage.FileName);
                string uniqueName = "rest_" + Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileName;
                string dirPath = Server.MapPath("~/Content/images");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                string targetPath = Path.Combine(dirPath, uniqueName);
                bannerImage.SaveAs(targetPath);
                model.ImageUrl = "/Content/images/" + uniqueName;
            }

            string message;
            if (_restRepo.Update(model, out message))
            {
                Session["RestaurantName"] = model.RestaurantName;
                TempData["Success"] = "Restaurant profile updated successfully.";
                return RedirectToAction("Details");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }
    }
}
