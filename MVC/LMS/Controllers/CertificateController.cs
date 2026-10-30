using System.Web.Mvc;
using LMS.Data;
using LMS.Services;

namespace LMS.Controllers
{
    public class CertificateController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly CertificateService _certificateService;

        public CertificateController()
        {
            _certificateService = new CertificateService(_db);
        }

        // GET: Certificate/ViewCertificate/5
        public ActionResult ViewCertificate(int id)
        {
            var model = _certificateService.GetCertificateViewModel(id);
            if (model == null) return HttpNotFound();

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
