using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.DAL;

namespace FoodArrival.Controllers
{
    public class HomeController : Controller
    {
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();

        public ActionResult Index()
        {
            var featuredItems = _foodRepo.GetAvailable().Take(4).ToList();
            return View(featuredItems);
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
