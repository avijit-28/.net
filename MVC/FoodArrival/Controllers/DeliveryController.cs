using System;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    [Authorize(Roles = "DeliveryAgent")]
    public class DeliveryController : Controller
    {
        public ActionResult Index(bool includeCompleted = false)
        {
            return View(new DeliveryDashboardViewModel
            {
                IncludeCompleted = includeCompleted,
                Assignments = new FoodRepository().GetDeliveries(Int32.Parse(User.Identity.Name), includeCompleted)
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Status(long orderId, string status)
        {
            if (!new[] { "PickedUp", "Delivered", "Failed" }.Contains(status)) return new HttpStatusCodeResult(400);
            try { new FoodRepository().SetDeliveryStatus(Int32.Parse(User.Identity.Name), orderId, status); TempData["Notice"] = "Delivery status updated."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }
    }
}
