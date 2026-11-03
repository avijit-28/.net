using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class RestaurantsController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(string city, string search)
        {
            try
            {
                ViewBag.City = city;
                ViewBag.Search = search;
                return View(new FoodRepository().SearchRestaurants(city, search));
            }
            catch (Exception ex)
            {
                ViewBag.DatabaseError = ex.Message;
                return View(new List<RestaurantCard>());
            }
        }

        [AllowAnonymous]
        public ActionResult Details(int id, string category, bool vegetarianOnly = false)
        {
            try
            {
                var restaurant = new FoodRepository().GetRestaurant(id, category, vegetarianOnly);
                if (restaurant == null) return HttpNotFound();
                return View(restaurant);
            }
            catch (System.Data.SqlClient.SqlException) { return HttpNotFound(); }
        }
    }
}
