using System;
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

        private bool IsRestaurantAuthenticated()
        {
            return Session["RestaurantId"] != null;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var rest = _restRepo.Login(model.Email.Trim().ToLower(), model.Password);
            if (rest != null)
            {
                Session["RestaurantId"] = rest.RestaurantId;
                Session["RestaurantName"] = rest.RestaurantName;
                Session["RestaurantEmail"] = rest.Email;
                Session["UserRole"] = "Restaurant";

                FormsAuthentication.SetAuthCookie(rest.Email, false);
                TempData["Success"] = "Welcome, " + rest.RestaurantName + " Kitchen Partner!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid restaurant partner email or password.");
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
        public ActionResult Dashboard()
        {
            if (!IsRestaurantAuthenticated())
            {
                return RedirectToAction("Login");
            }

            int restId = Convert.ToInt32(Session["RestaurantId"]);
            var orders = _restRepo.GetOrders(restId);
            ViewBag.AvailableItems = _foodRepo.GetAvailable();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            if (!IsRestaurantAuthenticated())
            {
                return RedirectToAction("Login");
            }

            _restRepo.UpdateDeliveryStatus(orderId, status);

            // Trigger notification
            if (status == "ReadyForPickup")
            {
                _notifRepo.AddNotification("Rider", null, "Order Ready for Pickup!", "Order #FD" + (1000 + orderId) + " is prepared and ready for delivery pickup.", "Order", "/Rider/Dashboard");
                _notifRepo.AddNotification("Customer", null, "Your Food is Prepared!", "Order #FD" + (1000 + orderId) + " is hot & packed, waiting for rider pickup.", "Order", "/Order/History");
            }
            else if (status == "Preparing")
            {
                _notifRepo.AddNotification("Customer", null, "Kitchen Cooking Your Order", "Chef is currently preparing Order #FD" + (1000 + orderId) + " with fresh ingredients.", "Order", "/Order/History");
            }

            TempData["Success"] = "Order #FD" + (1000 + orderId) + " status updated to " + status + ".";
            return RedirectToAction("Dashboard");
        }
    }
}
