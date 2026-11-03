using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Attributes;
using FoodArrival.DAL;
using FoodArrival.Models;
using FoodArrival.Services;

namespace FoodArrival.Controllers
{
    public class PaymentController : Controller
    {
        private readonly OrderRepository _orderRepo = new OrderRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        private bool IsCustomerAuthenticated()
        {
            return Session["CustomerId"] != null;
        }

        [HttpGet]
        public ActionResult Gateway(int orderId)
        {
            if (!IsCustomerAuthenticated())
            {
                TempData["Error"] = "Please login to access the payment gateway.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Gateway", "Payment", new { orderId = orderId }) });
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            PaymentGatewayViewModel vm = null;

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT o.OrderId, o.OrderNumber, o.CustomerId, c.CustomerName, c.PhoneNumber, c.Email,
                       ISNULL(o.DeliveryAddress, c.Address) AS DeliveryAddress,
                       o.TotalAmount, o.DeliveryFee, o.PaymentStatus, o.PaymentMethod, o.OrderStatus
                FROM dbo.Orders o
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                WHERE o.OrderId = @OrderId", conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int orderCustId = Convert.ToInt32(reader["CustomerId"]);
                        if (orderCustId != customerId)
                        {
                            TempData["Error"] = "Order not found or unauthorized.";
                            return RedirectToAction("Index", "Food");
                        }

                        string payStatus = reader["PaymentStatus"] != DBNull.Value ? reader["PaymentStatus"].ToString() : "Pending";
                        if (string.Equals(payStatus, "Success", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(payStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Track", "OrderTracking", new { orderId = orderId });
                        }

                        vm = new PaymentGatewayViewModel
                        {
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            OrderNumber = reader["OrderNumber"] != DBNull.Value ? reader["OrderNumber"].ToString() : ("FR" + (100000 + orderId)),
                            CustomerId = orderCustId,
                            CustomerName = reader["CustomerName"].ToString(),
                            CustomerPhone = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "",
                            CustomerEmail = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                            DeliveryAddress = reader["DeliveryAddress"] != DBNull.Value ? reader["DeliveryAddress"].ToString() : "",
                            TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                            DeliveryFee = reader["DeliveryFee"] != DBNull.Value ? Convert.ToDecimal(reader["DeliveryFee"]) : 40.00m,
                            PlatformFee = 15.00m,
                            PaymentMethod = reader["PaymentMethod"] != DBNull.Value ? reader["PaymentMethod"].ToString() : "UPI",
                            DefaultUpiId = (Session["CustomerUsername"]?.ToString() ?? "user") + "@okaxis"
                        };
                        vm.Subtotal = Math.Max(0, vm.TotalAmount - vm.DeliveryFee - vm.PlatformFee);
                    }
                }
            }

            if (vm == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Food");
            }

            // Fetch order items for summary
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("dbo.sp_OrderDetails_GetByOrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vm.Items.Add(new OrderDetail
                        {
                            OrderDetailId = Convert.ToInt32(reader["OrderDetailId"]),
                            OrderId = orderId,
                            FoodItemId = Convert.ToInt32(reader["FoodItemId"]),
                            ItemName = reader["ItemName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                            RestaurantName = reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : "FoodReady Partner Kitchen"
                        });
                    }
                }
            }

            vm.TotalItemsCount = vm.Items.Sum(x => x.Quantity);
            vm.ItemsSummary = string.Join(", ", vm.Items.Select(x => $"{x.ItemName} x{x.Quantity}"));

            return View(vm);
        }

        [HttpPost]
        [ValidateCustomAntiForgeryToken]
        public ActionResult ProcessPayment(int orderId, string paymentMethod, string upiId, string cardNumber, bool simulateFailure = false)
        {
            if (!IsCustomerAuthenticated())
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Session expired. Please log in again." });
                return RedirectToAction("Login", "Account");
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            var order = _orderRepo.GetById(orderId);
            if (order == null || order.CustomerId != customerId)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Invalid order." });
                return RedirectToAction("Index", "Food");
            }

            string cleanMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "UPI" : paymentMethod.Trim();
            string txnRef = "TXN-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + orderId;

            // 1. SIMULATE FAILURE SCENARIO
            if (simulateFailure)
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.Transactions 
                    (OrderId, TransactionReference, Amount, PaymentMethod, PaymentStatus, GatewayResponse, CreatedAt)
                    VALUES 
                    (@OrderId, @TxnRef, @Amount, @Method, 'Failed', 'Bank Gateway Simulation: Transaction declined by customer issuing bank.', GETDATE())", conn))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@TxnRef", txnRef);
                    cmd.Parameters.AddWithValue("@Amount", order.TotalAmount);
                    cmd.Parameters.AddWithValue("@Method", cleanMethod);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Payment Failed: Transaction was simulated as declined by bank gateway. You can retry with another method."
                    });
                }

                TempData["Error"] = "Payment Failed: Transaction was declined. Please try again or choose another payment method.";
                return RedirectToAction("Gateway", new { orderId = orderId });
            }

            // 2. SUCCESSFUL PAYMENT
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // A. Update Orders table
                        using (var cmd = new SqlCommand(@"
                            UPDATE dbo.Orders 
                            SET PaymentStatus = 'Success',
                                OrderStatus = 'Accepted',
                                PaymentMethod = @Method,
                                DeliveryStatus = 'Pending'
                            WHERE OrderId = @OrderId", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Method", cleanMethod);
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        // B. Update OrderSubOrders
                        using (var cmd = new SqlCommand(@"
                            UPDATE dbo.OrderSubOrders 
                            SET PreparationStatus = 'Confirmed',
                                DeliveryStatus = 'Pending'
                            WHERE OrderId = @OrderId", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        // C. Insert Transaction Record
                        using (var cmd = new SqlCommand(@"
                            INSERT INTO dbo.Transactions 
                            (OrderId, TransactionReference, Amount, PaymentMethod, PaymentStatus, GatewayResponse, CreatedAt)
                            VALUES 
                            (@OrderId, @TxnRef, @Amount, @Method, 'Success', 'Payment Authorized 200 OK via FoodReady Gateway', GETDATE())", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                            cmd.Parameters.AddWithValue("@TxnRef", txnRef);
                            cmd.Parameters.AddWithValue("@Amount", order.TotalAmount);
                            cmd.Parameters.AddWithValue("@Method", cleanMethod);
                            cmd.ExecuteNonQuery();
                        }

                        // D. Insert Order Status History
                        using (var cmd = new SqlCommand(@"
                            INSERT INTO dbo.OrderStatusHistory 
                            (OrderId, Status, ChangedByUserId, ChangedByRole, Message, CreatedAt)
                            VALUES 
                            (@OrderId, 'Payment Confirmed', @CustomerId, 'Customer', @Message, GETDATE())", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.Parameters.AddWithValue("@Message", $"Payment of INR {order.TotalAmount:N0} successfully verified via {cleanMethod}. Reference: {txnRef}");
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        if (Request.IsAjaxRequest())
                            return Json(new { success = false, message = "Payment processing error: " + ex.Message });
                        TempData["Error"] = "Payment failed: " + ex.Message;
                        return RedirectToAction("Gateway", new { orderId = orderId });
                    }
                }
            }

            // 3. Clear customer active cart
            Session["Cart"] = new List<CartItem>();

            // 4. Trigger intelligent Rider Allocation
            var allocService = new RiderAllocationService();
            var allocResult = allocService.AllocateRiders(orderId);

            string orderNum = !string.IsNullOrEmpty(order.OrderNumber) ? order.OrderNumber : ("FR" + (100000 + orderId));

            // 5. Send Notifications
            _notifRepo.AddNotification("Customer", customerId, "Payment Confirmed!",
                $"Payment of INR {order.TotalAmount:N0} for Order #{orderNum} was successful. Food is being prepared!", "Payment", $"/OrderTracking/Track?orderId={orderId}");

            _notifRepo.AddNotification("Admin", null, "New Paid Order Received",
                $"Order #{orderNum} (INR {order.TotalAmount:N0}) paid via {cleanMethod}. Reference: {txnRef}", "Payment", "/Admin/Transactions");

            // 6. Return response
            string trackUrl = Url.Action("Track", "OrderTracking", new { orderId = orderId });
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    orderId = orderId,
                    transactionReference = txnRef,
                    message = "Payment authorized successfully!",
                    redirectUrl = trackUrl
                });
            }

            TempData["Success"] = "Payment successful! Your order has been placed.";
            return RedirectToAction("Track", "OrderTracking", new { orderId = orderId });
        }

        [HttpPost]
        [ValidateCustomAntiForgeryToken]
        public ActionResult CancelOrder(int orderId)
        {
            if (!IsCustomerAuthenticated()) return RedirectToAction("Login", "Account");

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            var order = _orderRepo.GetById(orderId);
            if (order != null && order.CustomerId == customerId && order.PaymentStatus == "Pending")
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.Orders SET OrderStatus = 'Cancelled', DeliveryStatus = 'Cancelled' WHERE OrderId = @OrderId", conn))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["Success"] = "Order cancelled successfully.";
            }

            return RedirectToAction("PlaceOrder", "Order");
        }
    }
}
