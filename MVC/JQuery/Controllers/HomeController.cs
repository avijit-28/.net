using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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
        /// Connects via connection string 'DBConn' in Web.config and executes
        /// the stored procedure dbo.sp_SearchCustomers.
        /// </summary>
        [HttpPost]
        public JsonResult AjaxMethod(string searchTerm = null)
        {
            // Execute the stored procedure dbo.sp_SearchCustomers
            List<Customer> customers = CustomerRepository.GetCustomers(searchTerm);

            return Json(customers);
        }

        /// <summary>
        /// Handles AJAX requests for the JSGrid component using stored procedures.
        /// </summary>
        [HttpPost]
        public JsonResult GetJsGridData(string filterName = "", string filterCountry = "")
        {
            string searchTerm = !string.IsNullOrWhiteSpace(filterName) ? filterName : filterCountry;
            List<Customer> customers = CustomerRepository.GetCustomers(searchTerm);

            return Json(customers);
        }

        /// <summary>
        /// Action method to insert a new customer via stored procedure dbo.sp_InsertCustomer.
        /// </summary>
        [HttpPost]
        public JsonResult AddCustomer(Customer customer)
        {
            if (customer == null || string.IsNullOrWhiteSpace(customer.ContactName))
            {
                return Json(new { success = false, message = "ContactName is required." });
            }

            int newId = CustomerRepository.InsertCustomer(customer);
            return Json(new { success = newId > 0, customerId = newId });
        }

        /// <summary>
        /// Returns high-level dataset metrics fetched live from the SQL Server database.
        /// </summary>
        [HttpPost]
        public JsonResult GetDashboardMetrics()
        {
            var customers = CustomerRepository.GetCustomers();

            var metrics = new
            {
                TotalCustomers = customers.Count,
                TotalCountries = customers.Select(c => c.Country).Distinct().Count(),
                TotalCities = customers.Select(c => c.City).Distinct().Count(),
                Status = "Online (SQL Server)",
                DataSource = "dbo.sp_SearchCustomers",
                ConnectionName = "DBConn",
                ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Json(metrics);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Stored Procedure Integration with jQuery DataTables & JSGrid.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Database Configuration & Technical Support.";
            return View();
        }
    }
}
