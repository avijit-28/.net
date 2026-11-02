using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JQuery.Models;

namespace JQuery.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the jQuery AJAX operation for jQuery DataTables.
        /// Fetches records from the Customers table using Entity Framework and returns a JsonResult.
        /// </summary>
        [HttpPost]
        public JsonResult AjaxMethod()
        {
            BikeStoresEntities entities = new BikeStoresEntities();
            List<Customer> customers = (from customer in entities.Customers
                                        select customer).ToList();

            return Json(customers);
        }

        /// <summary>
        /// Handles AJAX requests for JSGrid client-side component.
        /// Supports optional filtering by ContactName or Country.
        /// </summary>
        [HttpPost]
        public JsonResult GetJsGridData(string filterName = "", string filterCountry = "")
        {
            BikeStoresEntities entities = new BikeStoresEntities();
            var query = entities.Customers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filterName))
            {
                query = query.Where(c => c.ContactName != null && c.ContactName.IndexOf(filterName, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(filterCountry))
            {
                query = query.Where(c => c.Country != null && c.Country.IndexOf(filterCountry, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return Json(query.ToList());
        }

        /// <summary>
        /// Returns high-level dataset metrics for the executive dashboard overview cards.
        /// </summary>
        [HttpPost]
        public JsonResult GetDashboardMetrics()
        {
            BikeStoresEntities entities = new BikeStoresEntities();
            var customers = entities.Customers;

            var metrics = new
            {
                TotalCustomers = customers.Count,
                TotalCountries = customers.Select(c => c.Country).Distinct().Count(),
                TotalCities = customers.Select(c => c.City).Distinct().Count(),
                Status = "Online",
                ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Json(metrics);
        }

        public ActionResult About()
        {
            ViewBag.Message = "jQuery DataTables and JSGrid in AJAX - Technical Reference.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Developer Support & Documentation.";
            return View();
        }
    }
}
