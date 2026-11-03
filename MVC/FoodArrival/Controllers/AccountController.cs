using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class AccountController : Controller
    {
        private readonly CustomerRepository _customerRepo = new CustomerRepository();

        [HttpGet]
        public ActionResult Register()
        {
            if (Session["CustomerId"] != null)
            {
                return RedirectToAction("Index", "Food");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = new Customer
            {
                CustomerName = model.CustomerName,
                Email = model.Email.Trim().ToLower(),
                PasswordHash = model.Password, // matches DB plain-text/hash convention
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };

            string message;
            int customerId;
            if (_customerRepo.Insert(customer, out message, out customerId))
            {
                // Auto sign in
                customer.CustomerId = customerId;
                SetUserSession(customer);

                FormsAuthentication.SetAuthCookie(customer.Email, false);
                TempData["Success"] = "Welcome to FoodReady, " + customer.CustomerName + "! Your account has been created.";
                return RedirectToAction("Index", "Food");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Session["CustomerId"] != null)
            {
                return RedirectToAction("Index", "Food");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = _customerRepo.Login(model.Email.Trim().ToLower(), model.Password);
            if (customer != null)
            {
                SetUserSession(customer);

                var ticket = new FormsAuthenticationTicket(
                    1,
                    customer.Email,
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    model.RememberMe,
                    "Customer",
                    FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                {
                    HttpOnly = true
                };
                if (model.RememberMe)
                {
                    cookie.Expires = ticket.Expiration;
                }
                Response.Cookies.Add(cookie);

                TempData["Success"] = "Welcome back, " + customer.CustomerName + "!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Food");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public new ActionResult Profile()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("Profile") });
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            var customer = _customerRepo.GetById(customerId);
            if (customer == null)
            {
                return RedirectToAction("Logout");
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Customer model)
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login");
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            model.CustomerId = customerId;

            string message;
            if (_customerRepo.Update(model, out message))
            {
                Session["CustomerName"] = model.CustomerName;
                Session["CustomerPhone"] = model.PhoneNumber;
                Session["CustomerAddress"] = model.Address;
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        private void SetUserSession(Customer customer)
        {
            Session["CustomerId"] = customer.CustomerId;
            Session["CustomerName"] = customer.CustomerName;
            Session["CustomerEmail"] = customer.Email;
            Session["CustomerPhone"] = customer.PhoneNumber;
            Session["CustomerAddress"] = customer.Address;
            Session["UserRole"] = "Customer";
        }
    }
}
