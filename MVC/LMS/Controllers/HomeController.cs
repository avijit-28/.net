using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMS.Data;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var featuredCourses = _db.Courses
                .Include(c => c.Category)
                .Where(c => c.Status == "Published")
                .OrderByDescending(c => c.Rating)
                .ThenByDescending(c => c.TotalStudents)
                .Take(6)
                .ToList();

            var categories = _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .Take(8)
                .ToList();

            ViewBag.TotalCourses = _db.Courses.Count(c => c.Status == "Published");
            ViewBag.TotalStudents = _db.Enrollments.Select(e => e.StudentId).Distinct().Count();
            ViewBag.TotalCertificates = _db.Certificates.Count();
            ViewBag.Categories = categories;

            return View(featuredCourses);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Empowering learners worldwide through modern interactive education.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Get in touch with our admissions and support team.";
            return View();
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