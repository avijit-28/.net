using Microsoft.AspNetCore.Mvc;

namespace WebApiCoreMVC.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
