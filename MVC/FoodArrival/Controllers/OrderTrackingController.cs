using System;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Models;
using FoodArrival.Services;

namespace FoodArrival.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly IOrderTrackingService _trackingService = new OrderTrackingService();

        private int? GetCurrentCustomerId()
        {
            if (Session["CustomerId"] != null)
            {
                int id;
                if (int.TryParse(Session["CustomerId"].ToString(), out id))
                    return id;
            }
            return null;
        }

        [HttpGet]
        public ActionResult Track(int? orderId)
        {
            int? customerId = GetCurrentCustomerId();
            if (!customerId.HasValue)
            {
                TempData["Error"] = "Please login to track your orders.";
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }

            // If orderId is omitted, find latest order for this customer
            if (!orderId.HasValue || orderId.Value <= 0)
            {
                var orders = _trackingService.GetCustomerOrders(customerId.Value);
                if (orders.Any())
                {
                    orderId = orders.First().OrderId;
                }
                else
                {
                    TempData["Error"] = "You have no active orders to track. Place your first order!";
                    return RedirectToAction("Index", "Food");
                }
            }

            // Security Authorization Requirement:
            // Customer A must only be able to see Customer A's orders.
            var trackingVm = _trackingService.GetTrackingDetails(orderId.Value, customerId.Value);
            if (trackingVm == null)
            {
                TempData["Error"] = "Order not found or you are not authorized to track this order.";
                return RedirectToAction("History", "Order");
            }

            return View(trackingVm);
        }

        [HttpGet]
        public ActionResult LiveStatus(int orderId)
        {
            int? customerId = GetCurrentCustomerId();
            if (!customerId.HasValue)
            {
                return Json(new { success = false, message = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

            var vm = _trackingService.GetTrackingDetails(orderId, customerId.Value);
            if (vm == null)
            {
                return Json(new { success = false, message = "Forbidden" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                orderId = vm.OrderId,
                orderNumber = vm.OrderNumber,
                status = vm.OverallStatus,
                isDelivered = vm.IsDelivered,
                isDelayed = vm.IsDelayed,
                delayMinutes = vm.DelayMinutes,
                etaMinutes = vm.OverallETAMinutes,
                subOrders = vm.SubOrders.Select(s => new
                {
                    subOrderId = s.SubOrderId,
                    restaurantName = s.RestaurantName,
                    prepStatus = s.PreparationStatus,
                    deliveryStatus = s.DeliveryStatus,
                    riderName = s.RiderName ?? "Allocating Rider",
                    riderVehicle = s.RiderVehicle ?? "Bike",
                    minutesToReady = s.MinutesToReady,
                    minutesToDelivery = s.MinutesToDelivery
                }),
                timeline = vm.TimelineSteps.Select(t => new
                {
                    step = t.StepNumber,
                    title = t.Title,
                    state = t.State,
                    time = t.TimeDisplay
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int orderId, string reason)
        {
            int? customerId = GetCurrentCustomerId();
            if (!customerId.HasValue) return RedirectToAction("Login", "Account");

            if (_trackingService.CancelOrder(orderId, customerId.Value, reason))
            {
                TempData["Success"] = "Your order #FR" + (100000 + orderId) + " was cancelled successfully.";
            }
            else
            {
                TempData["Error"] = "Order could not be cancelled. Kitchen preparation has already commenced.";
            }

            return RedirectToAction("Track", new { orderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(OrderRating model)
        {
            int? customerId = GetCurrentCustomerId();
            if (!customerId.HasValue) return RedirectToAction("Login", "Account");

            model.CustomerId = customerId.Value;
            if (_trackingService.SubmitRating(model))
            {
                TempData["Success"] = "Thank you for your valuable feedback! Ratings saved.";
            }
            else
            {
                TempData["Error"] = "Unable to record rating at this time.";
            }

            return RedirectToAction("Track", new { orderId = model.OrderId });
        }
    }
}
