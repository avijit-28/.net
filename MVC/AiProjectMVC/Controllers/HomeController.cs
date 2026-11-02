using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AiProjectMVC.Models;

namespace AiProjectMVC.Controllers
{
    /// <summary>
    /// Demonstrates Authorization, Custom Role Filters, and Authentication Filters
    /// with an easy-to-understand, interactive workflow.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// Security & Filters Center: Clear visual dashboard of all filters.
        /// Accessible to all authenticated users.
        /// </summary>
        public ActionResult Index()
        {
            try
            {
                string logPath = Server.MapPath("~/Log/Log.txt");
                if (System.IO.File.Exists(logPath))
                {
                    var lines = System.IO.File.ReadAllLines(logPath);
                    ViewBag.RecentLogs = lines.Reverse().Take(8).Reverse().ToList();
                }
                else
                {
                    ViewBag.RecentLogs = new List<string> { "Audit log file initialized. Click any action to see live entries." };
                }
            }
            catch
            {
                ViewBag.RecentLogs = new List<string>();
            }

            return View();
        }

        /// <summary>
        /// 1. Public / Non-Secure Endpoint: Decorated with [AllowAnonymous].
        /// Accessible by anyone (even guests not logged in).
        /// </summary>
        [AllowAnonymous]
        public ActionResult NonSecureMethod()
        {
            return View();
        }

        /// <summary>
        /// 2. Protected / Secure Endpoint: Decorated with [Authorize].
        /// Requires login. Unauthenticated visitors are automatically redirected to Login.
        /// </summary>
        public ActionResult SecureMethod()
        {
            return View();
        }

        /// <summary>
        /// 3. Role-Based Endpoint: Restricted to USER / Employee role.
        /// </summary>
        [AuthorizeRole(Role.USER, Role.EMPLOYEE, Role.SUPERADMIN)]
        public ActionResult UserPage()
        {
            ViewBag.Message = "User & Employee Exclusive Workspace";
            return View();
        }

        /// <summary>
        /// 4. Admin Role Endpoint: Restricted to ADMIN.
        /// Non-admin accounts will be intercepted and sent to AccessDenied.
        /// </summary>
        [AuthorizeRole(Role.ADMIN, Role.SUPERADMIN)]
        public ActionResult Contact()
        {
            ViewBag.Message = "Administrator Operations & Support Channel";
            return View();
        }

        /// <summary>
        /// 5. SuperAdmin Role Endpoint: Restricted exclusively to SUPERADMIN.
        /// Standard Admin and Users are redirected to AccessDenied.
        /// </summary>
        [AuthorizeRole(Role.SUPERADMIN)]
        public ActionResult About()
        {
            ViewBag.Message = "SuperAdmin Exclusive Master Control Panel";
            return View();
        }
    }
}
