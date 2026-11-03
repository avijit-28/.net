using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class HomeController : Controller
    {
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

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

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact our 24/7 FoodReady customer support team.";
            return View();
        }
    }
}
