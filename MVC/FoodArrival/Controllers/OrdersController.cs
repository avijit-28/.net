using System;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrdersController : Controller
    {
        public ActionResult Index() { return View(new FoodRepository().GetCustomerOrders(Int32.Parse(User.Identity.Name))); }

        public ActionResult Details(long id)
        {
            var order = new FoodRepository().GetCustomerOrder(Int32.Parse(User.Identity.Name), id);
            return order == null ? (ActionResult)HttpNotFound() : View(order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Review(long orderId, byte rating, string comment)
        {
            try
            {
                new FoodRepository().CreateReview(Int32.Parse(User.Identity.Name), orderId, rating, comment);
                TempData["Notice"] = "Thanks for sharing your review.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
