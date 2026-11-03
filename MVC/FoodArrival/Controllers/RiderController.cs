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
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        private bool IsRiderAuthenticated()
        {
            return Session["RiderId"] != null;
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
            if (!ModelState.IsValid)
            {
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

            ModelState.AddModelError("", "Invalid rider email or password.");
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
            ViewBag.AvailableDeliveries = _riderRepo.GetAvailableDeliveries();
            ViewBag.Rider = _riderRepo.GetById(riderId);
            var myDeliveries = _riderRepo.GetMyDeliveries(riderId);
            return View(myDeliveries);
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
                _notifRepo.AddNotification("Customer", null, "Rider On the Way!", riderName + " picked up your Order #FD" + (1000 + orderId) + " and is heading to your address.", "Order", "/Order/History");
                _notifRepo.AddNotification("Admin", null, "Delivery Dispatched", "Order #FD" + (1000 + orderId) + " is out for delivery with " + riderName + ".", "Order", "/Admin/Orders");
                TempData["Success"] = "You accepted delivery for Order #FD" + (1000 + orderId) + ". Drive safe!";
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
                _notifRepo.AddNotification("Customer", null, "Order Delivered!", "Your Order #FD" + (1000 + orderId) + " has been delivered. Enjoy your delicious food!", "Order", "/Order/History");
                _notifRepo.AddNotification("Admin", null, "Order Completed", "Order #FD" + (1000 + orderId) + " delivered successfully.", "Order", "/Admin/Orders?tab=accepted");
                TempData["Success"] = "Order #FD" + (1000 + orderId) + " marked as DELIVERED. Great job!";
            }
            else
            {
                TempData["Error"] = "Could not update delivery status.";
            }

            return RedirectToAction("Dashboard");
        }
    }
}
