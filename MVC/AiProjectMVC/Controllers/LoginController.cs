using System;
using System.Web.Mvc;

namespace AiProjectMVC.Controllers
{
    /// <summary>
    /// LoginController supporting the dedicated /Login/Login endpoint as specified in MVC authorization slides.
    /// Redirects or handles login requests in coordination with AccountController.
    /// </summary>
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = returnUrl });
        }
    }
}
