using System;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        public ActionResult Index() { return View(new FoodRepository().GetCustomerOrders(Int32.Parse(User.Identity.Name))); }

        public ActionResult Details(long id)
        {
            var order = new FoodRepository().GetCustomerOrder(Int32.Parse(User.Identity.Name), id);
            return order == null ? (ActionResult)HttpNotFound() : View(order);
        }
    }
}
