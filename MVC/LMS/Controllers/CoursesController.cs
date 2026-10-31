using System;
using System.Linq;
using System.Web.Mvc;
using LMS.Data;
using LMS.Models;
using LMS.Services;
using LMS.ViewModels;
using Microsoft.AspNet.Identity;

namespace LMS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;
        private readonly PaymentService _paymentService;

        public CoursesController()
        {
            _courseService = new CourseService(_db);
            _enrollmentService = new EnrollmentService(_db);
            _paymentService = new PaymentService(_db);
        }

        // GET: Courses
        public ActionResult Index(string search, int? categoryId, string level, string priceType, string sortBy, int page = 1)
        {
            var model = _courseService.GetFilteredCourses(search, categoryId, level, priceType, sortBy, page, 9);
            return View(model);
        }

        // GET: Courses/Details/5
        public ActionResult Details(int id)
        {
            string studentId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            var model = _courseService.GetCourseDetail(id, studentId);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Courses/Enroll
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public ActionResult Enroll(int courseId)
        {
            string studentId = User.Identity.GetUserId();
            var course = _db.Courses.Find(courseId);
            if (course == null) return HttpNotFound();

            if (course.Price > 0)
            {
                // Must go through payment flow
                return RedirectToAction("Purchase", new { courseId });
            }

            bool success = _enrollmentService.EnrollStudent(studentId, courseId, out string errorMessage);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Details", new { id = courseId });
            }

            TempData["SuccessMessage"] = "Successfully enrolled in the course! Start learning now.";
            return RedirectToAction("Index", "Learning", new { courseId });
        }

        // GET: Courses/Purchase
        [Authorize(Roles = "Student")]
        public ActionResult Purchase(int courseId)
        {
            string studentId = User.Identity.GetUserId();
            var course = _db.Courses.Find(courseId);
            if (course == null) return HttpNotFound();

            if (_enrollmentService.IsEnrolled(studentId, courseId))
            {
                TempData["InfoMessage"] = "You are already enrolled in this course.";
                return RedirectToAction("Index", "Learning", new { courseId });
            }

            return View(course);
        }

        // POST: Courses/Purchase
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int courseId, string paymentMethod)
        {
            string studentId = User.Identity.GetUserId();
            bool success = _paymentService.ProcessCoursePayment(studentId, courseId, paymentMethod, out string transactionId, out string error);

            if (!success)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("Purchase", new { courseId });
            }

            TempData["SuccessMessage"] = $"Payment successful! Transaction ID: {transactionId}. Welcome to the course!";
            return RedirectToAction("Index", "Learning", new { courseId });
        }

        // GET: Courses/Payment
        [Authorize(Roles = "Student")]
        public ActionResult Payment(int courseId)
        {
            string studentId = User.Identity.GetUserId();
            var course = _db.Courses.Find(courseId);
            if (course == null) return HttpNotFound();

            if (_enrollmentService.IsEnrolled(studentId, courseId))
            {
                TempData["InfoMessage"] = "You are already enrolled in this course.";
                return RedirectToAction("Index", "Learning", new { courseId });
            }

            return View(course);
        }

        // POST: Courses/Payment
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(int courseId, string paymentMethod)
        {
            string studentId = User.Identity.GetUserId();
            bool success = _paymentService.ProcessCoursePayment(studentId, courseId, paymentMethod, out string transactionId, out string error);

            if (!success)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("Payment", new { courseId });
            }

            TempData["SuccessMessage"] = $"Payment successful! Transaction ID: {transactionId}. Welcome to the course!";
            return RedirectToAction("Index", "Learning", new { courseId });
        }

        // POST: Courses/AddReview
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(CourseReviewInputModel model)
        {
            string studentId = User.Identity.GetUserId();
            var course = _db.Courses.Find(model.CourseId);
            if (course == null) return HttpNotFound();

            // Verify enrollment
            if (!_enrollmentService.IsEnrolled(studentId, model.CourseId))
            {
                TempData["ErrorMessage"] = "Only enrolled students can review this course.";
                return RedirectToAction("Details", new { id = model.CourseId });
            }

            var existing = _db.CourseReviews.FirstOrDefault(r => r.CourseId == model.CourseId && r.StudentId == studentId);
            if (existing != null)
            {
                existing.Rating = model.Rating;
                existing.ReviewText = model.ReviewText;
                existing.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                _db.CourseReviews.Add(new CourseReview
                {
                    CourseId = model.CourseId,
                    StudentId = studentId,
                    Rating = model.Rating,
                    ReviewText = model.ReviewText,
                    CreatedDate = DateTime.UtcNow
                });
            }
            _db.SaveChanges();

            // Recalculate average rating
            var reviews = _db.CourseReviews.Where(r => r.CourseId == model.CourseId).ToList();
            if (reviews.Any())
            {
                course.Rating = Math.Round((decimal)reviews.Average(r => r.Rating), 2);
                _db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Thank you! Your review has been submitted.";
            return RedirectToAction("Details", new { id = model.CourseId });
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
