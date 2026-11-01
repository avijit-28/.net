using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LMS.Data;
using LMS.Models;
using LMS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace LMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Prevent infinite redirect loop when an authenticated user lacks permissions for returnUrl
                return RedirectToLocal(null);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email?.Trim();
            var result = await SignInManager.PasswordSignInAsync(email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindByEmailAsync(email) ?? await UserManager.FindByNameAsync(email);
                    if (user != null && !user.IsApproved)
                    {
                        // User credentials are correct, but account is NOT yet approved by Admin!
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                        if (string.Equals(user.ApprovalStatus, "Declined", StringComparison.OrdinalIgnoreCase))
                        {
                            TempData["ErrorMessage"] = !string.IsNullOrWhiteSpace(user.RejectionReason)
                                ? $"Your registration application was declined by the administrator. Reason: {user.RejectionReason}"
                                : "Your registration application was declined by the administrator. Please contact support.";
                        }
                        else
                        {
                            TempData["InfoMessage"] = "Your instructor account is currently pending administrator verification. You will be able to sign in once approved.";
                        }

                        // Always redirect unapproved accounts to PendingApproval with their email
                        return RedirectToAction("PendingApproval", new { email = user.Email });
                    }
                    return await RedirectToUserDashboardAsync(user, returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fullName = model.FullName?.Trim();
                string email = model.Email?.Trim();
                string bio = model.Bio?.Trim();

                // Disallow registering as Admin
                bool isInstructor = string.Equals(model.Role, "Instructor", StringComparison.OrdinalIgnoreCase);
                string chosenRole = isInstructor ? "Instructor" : "Student";

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    Bio = bio,
                    CreatedDate = DateTime.UtcNow,
                    RegisteredDate = DateTime.UtcNow,
                    IsApproved = !isInstructor, // Students approved immediately! Instructors default to Pending
                    ApprovalStatus = isInstructor ? "Pending" : "Approved",
                    RequestedRole = chosenRole
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (isInstructor)
                    {
                        // Do NOT auto sign-in unapproved teachers!
                        // Dispatch an administrative Notification to all Admin users
                        await NotifyAdminsOfNewRegistrationAsync(user);

                        // Set success message and immediately redirect to the dedicated waiting/approval screen
                        TempData["SuccessMessage"] = "Your educator registration has been submitted successfully and is awaiting administrator verification.";
                        return RedirectToAction("PendingApproval", new { email = user.Email });
                    }
                    else
                    {
                        // Student is approved immediately! Assign Student role and sign in
                        await UserManager.AddToRoleAsync(user.Id, "Student");
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index", "Student");
                    }
                }
                AddErrors(result);
            }

            return View(model);
        }

        // GET: /Account/PendingApproval
        [AllowAnonymous]
        public async Task<ActionResult> PendingApproval(string email)
        {
            if (string.IsNullOrEmpty(email) && User.Identity.IsAuthenticated)
            {
                email = User.Identity.GetUserName();
            }

            PendingApprovalViewModel model = null;
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Trim();
                var user = await UserManager.FindByEmailAsync(email) ?? await UserManager.FindByNameAsync(email);
                if (user != null)
                {
                    // If user was already approved and is authenticated, redirect to user dashboard
                    if (user.IsApproved && User.Identity.IsAuthenticated)
                    {
                        return await RedirectToUserDashboardAsync(user, null);
                    }

                    model = new PendingApprovalViewModel
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        RequestedRole = !string.IsNullOrEmpty(user.RequestedRole) ? user.RequestedRole : "Instructor",
                        RegisteredDate = user.RegisteredDate != default(DateTime) ? user.RegisteredDate : user.CreatedDate,
                        ApprovalStatus = user.ApprovalStatus ?? "Pending",
                        RejectionReason = user.RejectionReason
                    };
                }
            }

            if (model == null)
            {
                model = new PendingApprovalViewModel
                {
                    FullName = "Valued Educator",
                    Email = email ?? "your-email@example.com",
                    RequestedRole = "Instructor",
                    RegisteredDate = DateTime.UtcNow,
                    ApprovalStatus = "Pending"
                };
            }

            return View(model);
        }

        private async Task NotifyAdminsOfNewRegistrationAsync(ApplicationUser newUser)
        {
            try
            {
                var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    var adminUserIds = await _db.Database.SqlQuery<string>(
                        "SELECT UserId FROM AspNetUserRoles WHERE RoleId = @p0", adminRole.Id
                    ).ToListAsync();

                    bool isInstructor = string.Equals(newUser.RequestedRole, "Instructor", StringComparison.OrdinalIgnoreCase);
                    string title = isInstructor ? "New Instructor Application Pending Review" : "New Registration Pending Approval";
                    string message = isInstructor
                        ? $"A new Instructor application has been submitted by '{newUser.FullName}' ({newUser.Email}) and is awaiting administrator verification."
                        : $"A new {newUser.RequestedRole} '{newUser.FullName}' ({newUser.Email}) has registered and is awaiting your review and approval.";
                    string notifType = isInstructor ? "InstructorApproval" : "AccountApproval";

                    foreach (var adminId in adminUserIds)
                    {
                        _db.Notifications.Add(new Notification
                        {
                            UserId = adminId,
                            Title = title,
                            Message = message,
                            NotificationType = notifType,
                            IsRead = false,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                // Suppress notification delivery error so registration flow completes safely
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        public new async Task<ActionResult> Profile()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return HttpNotFound();

            var roles = await UserManager.GetRolesAsync(user.Id);

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                Role = roles.FirstOrDefault() ?? "Student"
            };

            return View(model);
        }

        // POST: /Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new async Task<ActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return HttpNotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;
            user.ProfilePicture = model.ProfilePicture;

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            AddErrors(result);
            return View(model);
        }

        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction("Profile");
            }

            AddErrors(result);
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private async Task<ActionResult> RedirectToUserDashboardAsync(ApplicationUser user, string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user != null)
            {
                if (!user.IsApproved)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("PendingApproval", new { email = user.Email });
                }

                var roles = await UserManager.GetRolesAsync(user.Id);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (roles.Contains("Instructor"))
                {
                    return RedirectToAction("Index", "Instructor");
                }
            }

            return RedirectToAction("Index", "Student");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            string userId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                var roles = UserManager.GetRoles(userId);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (roles.Contains("Instructor"))
                {
                    return RedirectToAction("Index", "Instructor");
                }
            }

            return RedirectToAction("Index", "Student");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
