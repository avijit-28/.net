using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class FoodController : Controller
    {
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();

        public ActionResult Index(string category = "All items", string search = "")
        {
            List<FoodItem> items;

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = _foodRepo.Search(search);
                ViewBag.Search = search;
            }
            else if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All items", StringComparison.OrdinalIgnoreCase))
            {
                items = _foodRepo.GetByCategory(category);
            }
            else
            {
                items = _foodRepo.GetAvailable();
            }

            ViewBag.CurrentCategory = string.IsNullOrWhiteSpace(category) ? "All items" : category;
            ViewBag.Categories = new List<string> { "All items", "Pizza", "Burger", "Indian", "Chinese", "Drinks", "Snacks" };

            return View(items);
        }

        public ActionResult Details(int id)
        {
            var item = _foodRepo.GetById(id);
            if (item == null || !item.IsAvailable)
            {
                TempData["Error"] = "Food item not found or currently unavailable.";
                return RedirectToAction("Index");
            }
            return View(item);
        }
    }
}
