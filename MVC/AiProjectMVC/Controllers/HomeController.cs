using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiProjectMVC.Models;

namespace AiProjectMVC.Controllers
{
    /// <summary>
    /// Demonstrates Authorization Filter, Custom Role Authorization Filter,
    /// and Custom Authentication Filter as outlined in the MVC architecture.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// NonSecureMethod: Marked with [AllowAnonymous].
        /// Accessible by everyone (both authenticated and unauthenticated guests).
        /// </summary>
        [AllowAnonymous]
        public ActionResult NonSecureMethod()
        {
            return View();
        }

        /// <summary>
        /// SecureMethod: Requires authentication.
        /// Unauthenticated visitors are automatically redirected to Login.
        /// </summary>
        public ActionResult SecureMethod()
        {
            return View();
        }

        /// <summary>
        /// Main Hub / Demo Index: Open to everyone so guests and all roles can view the
        /// real-time Permission Matrix and use the 1-Click Role Switcher.
        /// </summary>
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// About: Restricted exclusively to SUPERADMIN.
        /// Regular ADMIN or USER accounts attempting to access this will be routed to AccessDenied.
        /// </summary>
        [AuthorizeRole(Role.SUPERADMIN)]
        public ActionResult About()
        {
            ViewBag.Message = "Super Administrator Exclusive Portal & System Description";
            return View();
        }

        /// <summary>
        /// Contact: Restricted exclusively to ADMIN.
        /// Non-admin accounts will be routed to AccessDenied.
        /// </summary>
        [AuthorizeRole(Role.ADMIN)]
        public ActionResult Contact()
        {
            ViewBag.Message = "Administrator Contact & Operations Channel";
            return View();
        }

        /// <summary>
        /// UserPage: Restricted to USER role.
        /// Demonstrates role-specific access for standard users.
        /// </summary>
        [AuthorizeRole(Role.USER)]
        public ActionResult UserPage()
        {
            ViewBag.Message = "Standard User Exclusive Workspace";
            return View();
        }
    }
}
