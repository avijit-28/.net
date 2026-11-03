using System;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    [Authorize(Roles = "Restaurant")]
    public class RestaurantDashboardController : Controller
    {
        public ActionResult Index()
        {
            var repo = new FoodRepository();
            var restaurants = repo.GetOwnedRestaurants(Int32.Parse(User.Identity.Name));
            if (!restaurants.Any()) return View(new RestaurantDashboardViewModel { Menu = new MenuItem[0], Orders = new OrderSummary[0], NewMenuItem = new MenuEditViewModel() });
            var restaurant = restaurants[0];
            return View(Build(repo, restaurant.Key, restaurant.Value, Int32.Parse(User.Identity.Name)));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveMenu(int restaurantId, MenuEditViewModel item)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Enter a name and a valid price."; return RedirectToAction("Index"); }
            try { new FoodRepository().SaveMenuItem(Int32.Parse(User.Identity.Name), restaurantId, item); TempData["Notice"] = "Menu item saved."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Availability(int menuItemId, bool isAvailable)
        {
            try { new FoodRepository().SetMenuAvailability(Int32.Parse(User.Identity.Name), menuItemId, isAvailable); TempData["Notice"] = "Menu availability updated."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrderStatus(long orderId, string status)
        {
            try { new FoodRepository().SetRestaurantOrderStatus(Int32.Parse(User.Identity.Name), orderId, status); TempData["Notice"] = "Order status updated."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        private static RestaurantDashboardViewModel Build(FoodRepository repo, int id, string name, int actorId)
        {
            return new RestaurantDashboardViewModel
            {
                RestaurantId = id, RestaurantName = name,
                Menu = repo.GetOwnedMenu(actorId, id),
                Orders = repo.GetRestaurantOrders(actorId, id),
                NewMenuItem = new MenuEditViewModel { IsAvailable = true }
            };
        }
    }
}
