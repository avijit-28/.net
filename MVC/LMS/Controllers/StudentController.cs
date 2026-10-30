using System.Linq;
using System.Web.Mvc;
using LMS.Data;
using LMS.Services;
using Microsoft.AspNet.Identity;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly DashboardService _dashboardService;
        private readonly EnrollmentService _enrollmentService;
        private readonly CertificateService _certificateService;
        private readonly QuizService _quizService;
        private readonly AssignmentService _assignmentService;

        public StudentController()
        {
            _dashboardService = new DashboardService(_db);
            _enrollmentService = new EnrollmentService(_db);
            _certificateService = new CertificateService(_db);
            _quizService = new QuizService(_db);
            _assignmentService = new AssignmentService(_db);
        }

        // GET: Student/Index
        public ActionResult Index()
        {
            string studentId = User.Identity.GetUserId();
            var model = _dashboardService.GetStudentDashboard(studentId);
            return View(model);
        }

        // GET: Student/MyCourses
        public ActionResult MyCourses()
        {
            string studentId = User.Identity.GetUserId();
            var enrollments = _enrollmentService.GetStudentEnrollments(studentId);
            return View(enrollments);
        }

        // GET: Student/Certificates
        public ActionResult Certificates()
        {
            string studentId = User.Identity.GetUserId();
            var certificates = _certificateService.GetStudentCertificates(studentId);
            return View(certificates);
        }

        // GET: Student/Quizzes
        public ActionResult Quizzes()
        {
            string studentId = User.Identity.GetUserId();
            var results = _quizService.GetStudentQuizHistory(studentId);
            return View(results);
        }

        // GET: Student/Assignments
        public ActionResult Assignments()
        {
            string studentId = User.Identity.GetUserId();
            var submissions = _assignmentService.GetStudentSubmissions(studentId);
            return View(submissions);
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
