using System;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class CartController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Total = CartSession.Total(Session);
            return View(CartSession.Get(Session));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(int restaurantId, int menuItemId, int quantity = 1)
        {
            if (quantity < 1 || quantity > 50) return new HttpStatusCodeResult(400, "Choose a quantity from 1 to 50.");
            try
            {
                var page = new FoodRepository().GetRestaurant(restaurantId, null, false);
                var item = page == null ? null : page.Menu.FirstOrDefault(x => x.MenuItemId == menuItemId);
                if (item == null) return HttpNotFound("This menu item is not available.");
                var cart = CartSession.Get(Session);
                if (cart.Any(x => x.RestaurantId != restaurantId))
                {
                    TempData["Error"] = "Your cart already contains items from another restaurant. Place that order first, then start a new cart.";
                    return RedirectToAction("Index", "Cart");
                }
                var line = cart.FirstOrDefault(x => x.MenuItemId == menuItemId);
                if (line == null) cart.Add(new CartLine { MenuItemId = item.MenuItemId, RestaurantId = restaurantId, RestaurantName = page.Restaurant.Name, Name = item.Name, UnitPrice = item.Price, Quantity = quantity });
                else line.Quantity = Math.Min(50, line.Quantity + quantity);
                TempData["Notice"] = "Added to your cart.";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; return RedirectToAction("Details", "Restaurants", new { id = restaurantId }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(int menuItemId, int quantity)
        {
            var line = CartSession.Get(Session).FirstOrDefault(x => x.MenuItemId == menuItemId);
            if (line != null)
            {
                if (quantity <= 0) CartSession.Get(Session).Remove(line);
                else line.Quantity = Math.Min(50, quantity);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Checkout()
        {
            if (!CartSession.Get(Session).Any()) return RedirectToAction("Index");
            return View(new CheckoutViewModel { PaymentMethod = "Cash" });
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var cart = CartSession.Get(Session);
            if (!cart.Any()) return RedirectToAction("Index");
            if (!new[] { "Cash", "Card", "UPI", "Other" }.Contains(model.PaymentMethod)) ModelState.AddModelError("PaymentMethod", "Choose a valid payment method.");
            if (!ModelState.IsValid) return View(model);
            try
            {
                var orderId = new FoodRepository().PlaceOrder(Int32.Parse(User.Identity.Name), cart[0].RestaurantId, model, cart);
                Session.Remove("FoodArrival.Cart");
                return RedirectToAction("Confirmation", new { id = orderId });
            }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        }

        [Authorize]
        public ActionResult Confirmation(long id)
        {
            var order = new FoodRepository().GetCustomerOrder(Int32.Parse(User.Identity.Name), id);
            if (order == null) return HttpNotFound();
            return View(order);
        }
    }
}
