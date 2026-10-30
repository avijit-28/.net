using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMS.Data;
using LMS.Models;
using LMS.Services;
using LMS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly DashboardService _dashboardService;

        public AdminController()
        {
            _dashboardService = new DashboardService(_db);
        }

        // GET: Admin/Index
        public ActionResult Index()
        {
            var model = _dashboardService.GetAdminDashboard();
            return View(model);
        }

        // GET: Admin/Users
        public ActionResult Users(string roleFilter = null)
        {
            var users = _db.Users.OrderByDescending(u => u.CreatedDate).ToList();
            var userRoles = _db.Database.SqlQuery<UserRoleMapping>(
                "SELECT ur.UserId, r.Name AS RoleName FROM AspNetUserRoles ur INNER JOIN AspNetRoles r ON ur.RoleId = r.Id"
            ).ToList();

            var list = users.Select(u => new UserManagementViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = userRoles.FirstOrDefault(ur => ur.UserId == u.Id)?.RoleName ?? "None",
                CreatedDate = u.CreatedDate,
                IsActive = !u.LockoutEnabled || (u.LockoutEndDateUtc == null || u.LockoutEndDateUtc < DateTime.UtcNow)
            }).ToList();

            if (!string.IsNullOrEmpty(roleFilter))
            {
                list = list.Where(u => string.Equals(u.Role, roleFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.CurrentRoleFilter = roleFilter;
            return View(list);
        }

        // GET: Admin/Students
        public ActionResult Students()
        {
            return RedirectToAction("Users", new { roleFilter = "Student" });
        }

        // GET: Admin/Instructors
        public ActionResult Instructors()
        {
            return RedirectToAction("Users", new { roleFilter = "Instructor" });
        }

        private class UserRoleMapping
        {
            public string UserId { get; set; }
            public string RoleName { get; set; }
        }

        // POST: Admin/ChangeRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeRole(string userId, string newRole)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var currentRoles = userManager.GetRoles(userId);
            if (currentRoles.Any())
            {
                userManager.RemoveFromRoles(userId, currentRoles.ToArray());
            }

            if (!string.IsNullOrEmpty(newRole))
            {
                userManager.AddToRole(userId, newRole);
            }

            TempData["SuccessMessage"] = "User role updated successfully.";
            return RedirectToAction("Users");
        }

        // GET: Admin/Categories
        public ActionResult Categories()
        {
            var categories = _db.Categories
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CourseCount = c.Courses.Count
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            return View(categories);
        }

        // GET: Admin/CreateCategory
        public ActionResult CreateCategory()
        {
            return View(new CategoryViewModel());
        }

        // POST: Admin/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                string catName = model.CategoryName.Trim().ToLower();
                if (_db.Categories.Any(c => c.CategoryName.ToLower() == catName))
                {
                    ModelState.AddModelError("CategoryName", "Category name already exists.");
                    return View(model);
                }

                var cat = new Category
                {
                    CategoryName = model.CategoryName,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                _db.Categories.Add(cat);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Categories");
            }

            return View(model);
        }

        // GET: Admin/EditCategory/5
        public ActionResult EditCategory(int id)
        {
            var cat = _db.Categories.Find(id);
            if (cat == null) return HttpNotFound();

            var model = new CategoryViewModel
            {
                CategoryId = cat.CategoryId,
                CategoryName = cat.CategoryName,
                Description = cat.Description,
                IsActive = cat.IsActive
            };

            return View(model);
        }

        // POST: Admin/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cat = _db.Categories.Find(model.CategoryId);
                if (cat == null) return HttpNotFound();

                string catName = model.CategoryName.Trim().ToLower();
                if (_db.Categories.Any(c => c.CategoryId != model.CategoryId && c.CategoryName.ToLower() == catName))
                {
                    ModelState.AddModelError("CategoryName", "Category name already exists.");
                    return View(model);
                }

                cat.CategoryName = model.CategoryName;
                cat.Description = model.Description;
                cat.IsActive = model.IsActive;

                _db.SaveChanges();
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Categories");
            }

            return View(model);
        }

        // POST: Admin/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
        {
            var cat = _db.Categories.Include(c => c.Courses).FirstOrDefault(c => c.CategoryId == id);
            if (cat == null) return HttpNotFound();

            if (cat.Courses.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete category containing courses. Deactivate it instead.";
                return RedirectToAction("Categories");
            }

            _db.Categories.Remove(cat);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Category removed successfully.";
            return RedirectToAction("Categories");
        }

        // GET: Admin/Courses
        public ActionResult Courses()
        {
            var courses = _db.Courses
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return View(courses);
        }

        // POST: Admin/UpdateCourseStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCourseStatus(int courseId, string status)
        {
            var course = _db.Courses.Find(courseId);
            if (course == null) return HttpNotFound();

            course.Status = status;
            if (status == "Published" && course.PublishedDate == null)
            {
                course.PublishedDate = DateTime.UtcNow;
            }
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Course '{course.Title}' status updated to {status}.";
            return RedirectToAction("Courses");
        }

        // GET: Admin/Enrollments
        public ActionResult Enrollments()
        {
            var enrollments = _db.Enrollments
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToList();

            return View(enrollments);
        }

        // GET: Admin/Payments
        public ActionResult Payments()
        {
            var payments = _db.Payments
                .Include(p => p.Course)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();

            return View(payments);
        }

        // GET: Admin/Announcements
        public ActionResult Announcements()
        {
            var announcements = _db.Announcements
                .OrderByDescending(a => a.CreatedDate)
                .ToList();

            return View(announcements);
        }

        // GET: Admin/CreateAnnouncement
        public ActionResult CreateAnnouncement()
        {
            return View(new AnnouncementFormViewModel());
        }

        // POST: Admin/CreateAnnouncement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnnouncement(AnnouncementFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var announcement = new Announcement
                {
                    Title = model.Title,
                    Message = model.Message,
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = model.IsActive
                };

                _db.Announcements.Add(announcement);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Announcement published successfully!";
                return RedirectToAction("Announcements");
            }

            return View(model);
        }

        // POST: Admin/DeleteAnnouncement/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAnnouncement(int id)
        {
            var a = _db.Announcements.Find(id);
            if (a != null)
            {
                _db.Announcements.Remove(a);
                _db.SaveChanges();
            }
            TempData["SuccessMessage"] = "Announcement deleted.";
            return RedirectToAction("Announcements");
        }

        // GET: Admin/Reports
        public ActionResult Reports()
        {
            var model = _dashboardService.GetAdminDashboard();
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
