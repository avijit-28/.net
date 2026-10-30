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

        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            // Load top demo accounts for quick testing
            try
            {
                var users = _userRepo.GetAllUsers();
                ViewBag.DemoUsers = users.Take(4).ToList();
            }
            catch
            {
                ViewBag.DemoUsers = new List<EmployeeUser>();
            }

            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
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
                var user = _userRepo.AuthenticateUser(model.UserNameOrEmail, model.Password);

                if (user != null)
                {
                    // Store user details in session
                    Session["CurrentUser"] = user;
                    Session["UserId"] = user.ID;
                    Session["UserName"] = user.UserName;
                    Session["FullName"] = user.Name;
                    Session["UserRole"] = user.Role;
                    Session["Email"] = user.Email;

                    // Set authentication cookie
                    FormsAuthentication.SetAuthCookie(user.UserName, model.RememberMe);

                    TempData["SuccessMessage"] = $"Welcome back, {user.Name}! Logged in as {user.Role}.";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Direct to Employee Dashboard
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

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
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
                ModelState.AddModelError("", "Failed to register user: " + ex.Message);
                return View(model);
            }
        }

        // GET or POST: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        private void LoadDemoUsers()
        {
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
