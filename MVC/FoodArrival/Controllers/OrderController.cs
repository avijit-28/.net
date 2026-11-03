using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepo = new OrderRepository();
        private readonly FoodItemRepository _foodRepo = new FoodItemRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        private List<CartItem> GetCart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        [HttpPost]
        public ActionResult AddToCart(int foodItemId, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            var food = _foodRepo.GetById(foodItemId);
            if (food == null || !food.IsAvailable)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Item not available." });
                }
                TempData["Error"] = "Item not available.";
                return RedirectToAction("Index", "Food");
            }

            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.FoodItemId == foodItemId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    FoodItemId = food.FoodItemId,
                    ItemName = food.ItemName,
                    Description = food.Description,
                    UnitPrice = food.Price,
                    Quantity = quantity,
                    ImageUrl = food.ImageUrl
                });
            }

            Session["Cart"] = cart;

            if (Request.IsAjaxRequest())
            {
                int totalCount = cart.Sum(x => x.Quantity);
                return Json(new { success = true, totalCount = totalCount, message = food.ItemName + " added to cart." });
            }

            TempData["Success"] = food.ItemName + " added to cart.";
            return RedirectToAction("PlaceOrder");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int foodItemId, int quantity)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.FoodItemId == foodItemId);

            if (existing != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(existing);
                }
                else
                {
                    existing.Quantity = quantity;
                }
            }

            Session["Cart"] = cart;

            if (Request.IsAjaxRequest())
            {
                decimal subtotal = cart.Sum(x => x.SubTotal);
                decimal deliveryFee = cart.Count > 0 ? 40.00m : 0.00m;
                decimal total = subtotal + deliveryFee;
                int totalCount = cart.Sum(x => x.Quantity);

                return Json(new
                {
                    success = true,
                    itemSubtotal = existing != null ? existing.SubTotal : 0,
                    subtotal = subtotal,
                    deliveryFee = deliveryFee,
                    totalAmount = total,
                    totalCount = totalCount
                });
            }

            return RedirectToAction("PlaceOrder");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int foodItemId)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.FoodItemId == foodItemId);
            if (existing != null)
            {
                cart.Remove(existing);
            }
            Session["Cart"] = cart;

            if (Request.IsAjaxRequest())
            {
                decimal subtotal = cart.Sum(x => x.SubTotal);
                decimal deliveryFee = cart.Count > 0 ? 40.00m : 0.00m;
                decimal total = subtotal + deliveryFee;
                int totalCount = cart.Sum(x => x.Quantity);

                return Json(new
                {
                    success = true,
                    subtotal = subtotal,
                    deliveryFee = deliveryFee,
                    totalAmount = total,
                    totalCount = totalCount
                });
            }

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction("PlaceOrder");
        }

        [HttpGet]
        public ActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PlaceOrder()
        {
            var cart = GetCart();
            decimal subtotal = cart.Sum(x => x.SubTotal);
            decimal deliveryFee = cart.Count > 0 ? 40.00m : 0.00m;

            var model = new CartViewModel
            {
                Items = cart,
                Subtotal = subtotal,
                DeliveryFee = deliveryFee,
                TotalAmount = subtotal + deliveryFee,
                CustomerName = Session["CustomerName"]?.ToString(),
                PhoneNumber = Session["CustomerPhone"]?.ToString(),
                DeliveryAddress = Session["CustomerAddress"]?.ToString()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(string deliveryAddress, string phoneNumber)
        {
            if (Session["CustomerId"] == null)
            {
                TempData["Error"] = "Please login to place your order.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("PlaceOrder", "Order") });
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty. Add food items before placing an order.";
                return RedirectToAction("Index", "Food");
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);

            // If delivery address / phone updated, update customer in DB
            if (!string.IsNullOrWhiteSpace(deliveryAddress) || !string.IsNullOrWhiteSpace(phoneNumber))
            {
                var cust = _customerRepo.GetById(customerId);
                if (cust != null)
                {
                    if (!string.IsNullOrWhiteSpace(deliveryAddress)) cust.Address = deliveryAddress;
                    if (!string.IsNullOrWhiteSpace(phoneNumber)) cust.PhoneNumber = phoneNumber;
                    string updateMsg;
                    _customerRepo.Update(cust, out updateMsg);
                    Session["CustomerAddress"] = cust.Address;
                    Session["CustomerPhone"] = cust.PhoneNumber;
                }
            }

            string message;
            int orderId;
            if (_orderRepo.CreateOrder(customerId, cart, out message, out orderId))
            {
                decimal orderTotal = cart.Sum(x => x.SubTotal);
                string customerName = Session["CustomerName"]?.ToString() ?? "Customer";

                // Broadcast notifications across roles
                _notifRepo.AddNotification("Customer", customerId, "Order Placed Successfully", "We received your Order #FD" + (1000 + orderId) + ". Currently Pending approval.", "Order", "/Order/History");
                _notifRepo.AddNotification("Restaurant", null, "New Order Received!", "Order #FD" + (1000 + orderId) + " placed by " + customerName + " (Total: ₹" + orderTotal.ToString("F0") + ").", "Order", "/Restaurant/Dashboard");
                _notifRepo.AddNotification("Admin", null, "New Pending Order", "Order #FD" + (1000 + orderId) + " is pending. Subject to 5-min auto-reject.", "Alert", "/Admin/Orders?tab=pending");

                Session["Cart"] = new List<CartItem>(); // Empty the cart
                TempData["Success"] = "Order #FD" + (1000 + orderId) + " placed successfully! Status: Pending.";
                return RedirectToAction("History");
            }

            TempData["Error"] = "Failed to place order: " + message;
            return RedirectToAction("PlaceOrder");
        }

        [HttpGet]
        public ActionResult History()
        {
            if (Session["CustomerId"] == null)
            {
                TempData["Error"] = "Please login to view your order history.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("History", "Order") });
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            var customer = _customerRepo.GetById(customerId);
            var orders = _orderRepo.GetByCustomer(customerId);

            ViewBag.Customer = customer;
            return View(orders);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            if (Session["CustomerId"] == null && Session["AdminId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = _orderRepo.GetDetails(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("History");
            }

            // If customer, verify ownership
            if (Session["CustomerId"] != null)
            {
                int currentCustId = Convert.ToInt32(Session["CustomerId"]);
                if (order.CustomerId != currentCustId && Session["AdminId"] == null)
                {
                    TempData["Error"] = "Unauthorized to view this order.";
                    return RedirectToAction("History");
                }
            }

            return View(order);
        }
    }
}
