using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AiProjectMVC.Models;
using AiProjectMVC.ViewModels;

namespace AiProjectMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmployeeUserRepo _userRepo = new EmployeeUserRepo();
        private readonly Repository _authRepo = new Repository();

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoadDemoUsers();
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                LoadDemoUsers();
                return View(model);
            }

            try
            {
                string inputIdentifier = (model.UserNameOrEmail ?? "").Trim();
                string inputPassword = (model.Password ?? "").Trim();

                // 1. Check in USERS table (PDF custom authentication / role demo accounts)
                USER pdfUser = null;
                try
                {
                    pdfUser = _authRepo.GetUserDetails(inputIdentifier, inputPassword);
                }
                catch
                {
                    // Continue to EmployeeUser if query fails
                }

                if (pdfUser != null)
                {
                    // Issue FormsAuthenticationTicket with Roles in UserData (Slides 18-19, 21)
                    FormsAuthentication.SetAuthCookie(pdfUser.UserName, model.RememberMe);
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        pdfUser.UserName,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        model.RememberMe,
                        pdfUser.Roles ?? "USER"
                    );
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    if (model.RememberMe)
                    {
                        authCookie.Expires = authTicket.Expiration;
                    }
                    Response.Cookies.Add(authCookie);

                    // Set Session variables
                    Session["CurrentUser"] = new EmployeeUser
                    {
                        ID = (int)pdfUser.UserID,
                        UserName = pdfUser.UserName,
                        Name = pdfUser.UserName.Contains("@") ? pdfUser.UserName.Split('@')[0] : pdfUser.UserName,
                        Role = pdfUser.Roles,
                        Email = pdfUser.UserName.Contains("@") ? pdfUser.UserName : ""
                    };
                    Session["UserId"] = pdfUser.UserID;
                    Session["UserName"] = pdfUser.UserName;
                    Session["FullName"] = pdfUser.UserName;
                    Session["UserRole"] = pdfUser.Roles;
                    Session["Email"] = pdfUser.UserName;

                    TempData["SuccessMessage"] = $"Logged in successfully as {pdfUser.UserName} (Roles: {pdfUser.Roles})!";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                // 2. Check in EmployeeUser table (Employee management accounts)
                var empUser = _userRepo.AuthenticateUser(inputIdentifier, inputPassword);
                if (empUser != null)
                {
                    // Issue FormsAuthenticationTicket with Role in UserData
                    FormsAuthentication.SetAuthCookie(empUser.UserName, model.RememberMe);
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        empUser.UserName,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        model.RememberMe,
                        empUser.Role ?? "Employee"
                    );
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    if (model.RememberMe)
                    {
                        authCookie.Expires = authTicket.Expiration;
                    }
                    Response.Cookies.Add(authCookie);

                    // Store user details in session
                    Session["CurrentUser"] = empUser;
                    Session["UserId"] = empUser.ID;
                    Session["UserName"] = empUser.UserName;
                    Session["FullName"] = empUser.Name;
                    Session["UserRole"] = empUser.Role;
                    Session["Email"] = empUser.Email;

                    TempData["SuccessMessage"] = $"Welcome back, {empUser.Name}! Logged in as {empUser.Role}.";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Dashboard", "Employee");
                }

                ModelState.AddModelError("", "Invalid Username/Email or Password. Please check your credentials.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during authentication: " + ex.Message);
            }

            LoadDemoUsers();
            return View(model);
        }

        // GET: /Account/AccessDenied
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (_userRepo.IsUsernameTaken(model.UserName))
                {
                    ModelState.AddModelError("UserName", "This username is already taken. Please choose another.");
                    return View(model);
                }

                if (_userRepo.IsEmailTaken(model.Email))
                {
                    ModelState.AddModelError("Email", "An account with this email address already exists.");
                    return View(model);
                }

                var newUser = new EmployeeUser
                {
                    UserName = model.UserName,
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = !string.IsNullOrWhiteSpace(model.Role) ? model.Role : "Employee",
                    Password = model.Password
                };

                int newId = _userRepo.RegisterUser(newUser);

                TempData["SuccessMessage"] = $"Account registered successfully! Welcome {newUser.Name}. You can now login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                string friendlyMessage = "Registration could not be completed. ";
                if (ex.Message.Contains("IX_") || ex.Message.ToLower().Contains("unique") || ex.Message.ToLower().Contains("duplicate"))
                {
                    friendlyMessage += "An account with the same username or email already exists.";
                }
                else if (ex.Message.ToLower().Contains("password") || ex.Message.ToLower().Contains("null"))
                {
                    friendlyMessage += "Please ensure all required fields, including password, are properly filled.";
                }
                else
                {
                    friendlyMessage += "Please check your inputs and try again.";
                }

                ModelState.AddModelError("", friendlyMessage);
                return View(model);
            }
        }

        // GET or POST: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            return Logout();
        }

        private void LoadDemoUsers()
        {
            try
            {
                ViewBag.PdfUsers = _authRepo.GetAllUsers();
            }
            catch
            {
                ViewBag.PdfUsers = new List<USER>();
            }

            try
            {
                var users = _userRepo.GetAllUsers();
                ViewBag.DemoUsers = users.Take(4).ToList();
            }
            catch
            {
                ViewBag.DemoUsers = new List<EmployeeUser>();
            }
        }
    }
}
