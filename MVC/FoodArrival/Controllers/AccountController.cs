using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class AccountController : Controller
    {
        private FoodRepository Repository { get { return new FoodRepository(); } }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            AccountRecord account;
            try { account = Repository.FindAccount(model.Email.Trim()); }
            catch (ConfigurationErrorsException)
            {
                ModelState.AddModelError("", "Database connection is not configured. Add the FoodDeliveryDB connection string in Web.config.");
                return View(model);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ModelState.AddModelError("", "FoodArrival could not connect to the database. Check the FoodDeliveryDB connection and run the database script.");
                return View(model);
            }
            if (account == null || !VerifyPassword(model.Password, account.PasswordHash))
            {
                ModelState.AddModelError("", "Email or password is incorrect.");
                return View(model);
            }
            var ticket = new FormsAuthenticationTicket(1, account.UserId.ToString(), DateTime.Now,
                DateTime.Now.AddHours(model.RememberMe ? 24 : 8), model.RememberMe, account.Role + "|" + (account.StaffType ?? ""));
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection,
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Add(cookie);
            if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);
            if (account.Role == "Staff" && account.StaffType == "Restaurant") return RedirectToAction("Index", "RestaurantDashboard");
            if (account.Role == "Staff" && account.StaffType == "DeliveryAgent") return RedirectToAction("Index", "Delivery");
            if (account.Role == "Admin") return RedirectToAction("Index", "Admin");
            return RedirectToAction("Index", "Restaurants");
        }

        [AllowAnonymous]
        public ActionResult Register() { return View(new RegisterViewModel { RequestedAccountType = "Customer" }); }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                if (!new[] { "Customer", "Staff" }.Contains(model.RequestedAccountType))
                    ModelState.AddModelError("RequestedAccountType", "Choose Customer or Staff.");
                if (model.RequestedAccountType == "Staff" && !new[] { "Restaurant", "DeliveryAgent" }.Contains(model.RequestedStaffType))
                    ModelState.AddModelError("RequestedStaffType", "Choose the staff area you work in.");
                if (!ModelState.IsValid) return View(model);
                var id = Repository.Register(model.FullName.Trim(), model.Email.Trim(), model.Phone.Trim(), HashPassword(model.Password),
                    model.RequestedAccountType, model.RequestedStaffType, model.RequestNote);
                var ticket = new FormsAuthenticationTicket(1, id.ToString(), DateTime.Now, DateTime.Now.AddHours(8), false, "Customer|");
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
                {
                    HttpOnly = true, Secure = Request.IsSecureConnection, SameSite = SameSiteMode.Lax
                });
                if (model.RequestedAccountType == "Staff")
                    TempData["Notice"] = "Your customer account is ready. Your " + model.RequestedStaffType + " staff request is pending administrator approval.";
                return RedirectToAction("Index", "Restaurants");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ModelState.AddModelError("", ex.Number == 2601 || ex.Number == 2627 ? "An account with that email already exists." : "The account could not be created. Check the database connection and try again.");
                return View(model);
            }
            catch (ConfigurationErrorsException)
            {
                ModelState.AddModelError("", "Database connection is not configured. Add the FoodDeliveryDB connection string in Web.config.");
                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Restaurants");
        }

        private static string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            const int iterations = 150000;
            byte[] hash;
            using (var derive = new Rfc2898DeriveBytes(password, salt, iterations)) hash = derive.GetBytes(32);
            return iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string stored)
        {
            try
            {
                var parts = stored.Split(':');
                var iterations = Int32.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);
                byte[] actual;
                using (var derive = new Rfc2898DeriveBytes(password, salt, iterations)) actual = derive.GetBytes(expected.Length);
                var difference = expected.Length ^ actual.Length;
                for (var i = 0; i < expected.Length && i < actual.Length; i++) difference |= expected[i] ^ actual[i];
                return difference == 0;
            }
            catch { return false; }
        }
    }
}
