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

        // GET: Certificate/Verify?certNumber=LMS-2026-123456
        [AllowAnonymous]
        public ActionResult Verify(string certNumber)
        {
            if (string.IsNullOrWhiteSpace(certNumber))
            {
                ViewBag.ErrorMessage = "Please provide a valid certificate number.";
                return View("VerifyCertificate", null);
            }

            var cert = _certificateService.GetCertificateByNumber(certNumber.Trim());
            if (cert == null)
            {
                ViewBag.ErrorMessage = $"Certificate #{certNumber} was not found or is invalid.";
                return View("VerifyCertificate", null);
            }

            var model = _certificateService.GetCertificateViewModel(cert.CertificateId);
            return View("ViewCertificate", model);
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
