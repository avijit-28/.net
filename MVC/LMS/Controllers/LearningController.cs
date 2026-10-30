using System.Web.Mvc;
using LMS.Data;
using LMS.Services;
using Microsoft.AspNet.Identity;

namespace LMS.Controllers
{
    [Authorize]
    public class LearningController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly LearningService _learningService;
        private readonly EnrollmentService _enrollmentService;

        public LearningController()
        {
            _learningService = new LearningService(_db);
            _enrollmentService = new EnrollmentService(_db);
        }

        // GET: Learning?courseId=1&lessonId=2
        public ActionResult Index(int courseId, int? lessonId = null)
        {
            string studentId = User.Identity.GetUserId();

            // Check if user is enrolled
            if (!_enrollmentService.IsEnrolled(studentId, courseId))
            {
                // Allow instructor of course or admin to preview
                var course = _db.Courses.Find(courseId);
                bool isInstructor = course != null && course.InstructorId == studentId;
                bool isAdmin = User.IsInRole("Admin");

                if (!isInstructor && !isAdmin)
                {
                    TempData["ErrorMessage"] = "You must be enrolled in this course to access the classroom.";
                    return RedirectToAction("Details", "Courses", new { id = courseId });
                }
            }

            var model = _learningService.GetLearningState(courseId, studentId, lessonId);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Learning/MarkComplete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkComplete(int lessonId)
        {
            string studentId = User.Identity.GetUserId();
            decimal progress = _learningService.MarkLessonComplete(studentId, lessonId, out bool isCourseCompleted);

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true, progress = progress, isCompleted = isCourseCompleted });
            }

            var lesson = _db.Lessons.Find(lessonId);
            int courseId = lesson != null ? lesson.Module.CourseId : 1;
            return RedirectToAction("Index", new { courseId, lessonId });
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
