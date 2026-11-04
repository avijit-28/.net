using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Services
{
    public class OrderTrackingService : IOrderTrackingService
    {
        public OrderTrackingViewModel GetTrackingDetails(int orderId, int customerId)
        {
            var vm = new OrderTrackingViewModel();

            // 1. Fetch Header & Validate Ownership
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("dbo.sp_OrderTracking_GetByOrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        // Authorization check: Order does not exist or does not belong to this customer!
                        return null;
                    }

                    vm.OrderId = Convert.ToInt32(reader["OrderId"]);
                    vm.OrderNumber = reader["OrderNumber"].ToString();
                    vm.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                    vm.CustomerName = reader["CustomerName"].ToString();
                    vm.CustomerPhone = reader["CustomerPhone"] != DBNull.Value ? reader["CustomerPhone"].ToString() : null;
                    vm.TotalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                    vm.DeliveryFee = Convert.ToDecimal(reader["DeliveryFee"]);
                    vm.DeliveryAddress = reader["DeliveryAddress"] != DBNull.Value ? reader["DeliveryAddress"].ToString() : "Home Delivery";
                    vm.PaymentStatus = reader["PaymentStatus"].ToString();
                    vm.PaymentMethod = reader["PaymentMethod"].ToString();
                    vm.OverallStatus = reader["OrderStatus"].ToString();
                    vm.DeliveryOtp = reader["DeliveryOtp"] != DBNull.Value ? reader["DeliveryOtp"].ToString() : null;
                    vm.OrderedAt = Convert.ToDateTime(reader["OrderedAt"]);
                    vm.EstimatedDeliveryTime = reader["EstimatedDeliveryTime"] != DBNull.Value
                        ? (DateTime?)Convert.ToDateTime(reader["EstimatedDeliveryTime"])
                        : vm.OrderedAt.AddMinutes(35);
                    vm.SubTotal = vm.TotalAmount - vm.DeliveryFee - vm.PlatformFee;
                    if (vm.SubTotal < 0) vm.SubTotal = vm.TotalAmount;
                }
            }

            // 2. Fetch Restaurant Sub-Orders
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("dbo.sp_OrderSubOrderTracking_GetByOrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sub = new OrderSubOrder
                        {
                            SubOrderId = Convert.ToInt32(reader["SubOrderId"]),
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                            RestaurantName = reader["RestaurantName"].ToString(),
                            RestaurantAddress = reader["RestaurantAddress"] != DBNull.Value ? reader["RestaurantAddress"].ToString() : null,
                            RestaurantPhone = reader["RestaurantPhone"] != DBNull.Value ? reader["RestaurantPhone"].ToString() : null,
                            Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                            RestaurantImage = reader["RestaurantImage"] != DBNull.Value ? reader["RestaurantImage"].ToString() : null,
                            RestaurantLatitude = Convert.ToDecimal(reader["RestaurantLatitude"]),
                            RestaurantLongitude = Convert.ToDecimal(reader["RestaurantLongitude"]),
                            SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                            PreparationStatus = reader["PreparationStatus"].ToString(),
                            EstimatedReadyAt = reader["EstimatedReadyAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["EstimatedReadyAt"]) : null,
                            RiderId = reader["RiderId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["RiderId"]) : null,
                            RiderName = reader["RiderName"] != DBNull.Value ? reader["RiderName"].ToString() : null,
                            RiderVehicle = reader["RiderVehicle"] != DBNull.Value ? reader["RiderVehicle"].ToString() : null,
                            RiderRating = reader["RiderRating"] != DBNull.Value ? Convert.ToDecimal(reader["RiderRating"]) : 4.8m,
                            RiderLatitude = reader["RiderLatitude"] != DBNull.Value ? Convert.ToDecimal(reader["RiderLatitude"]) : 22.582000m,
                            RiderLongitude = reader["RiderLongitude"] != DBNull.Value ? Convert.ToDecimal(reader["RiderLongitude"]) : 88.422000m,
                            DeliveryStatus = reader["DeliveryStatus"].ToString(),
                            PickupSequence = Convert.ToInt32(reader["PickupSequence"]),
                            DeliveredAt = reader["DeliveredAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["DeliveredAt"]) : null,
                            EstimatedDeliveryAt = reader["EstimatedDeliveryAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["EstimatedDeliveryAt"]) : null,
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                        vm.SubOrders.Add(sub);
                    }
                }
            }

            // 3. Fetch Order Items
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
                        var item = new OrderDetail
                        {
                            OrderDetailId = Convert.ToInt32(reader["OrderDetailId"]),
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            SubOrderId = reader["SubOrderId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["SubOrderId"]) : null,
                            FoodItemId = Convert.ToInt32(reader["FoodItemId"]),
                            ItemName = reader["ItemName"].ToString(),
                            Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                            ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "/Content/images/margherita.jpg",
                            Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "Snacks",
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                            RestaurantId = reader["RestaurantId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["RestaurantId"]) : null,
                            RestaurantName = reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : null
                        };
                        vm.OrderItems.Add(item);

                        // Attach to sub-order if matched
                        if (item.SubOrderId.HasValue)
                        {
                            var matchSub = vm.SubOrders.FirstOrDefault(s => s.SubOrderId == item.SubOrderId.Value);
                            if (matchSub != null) matchSub.Items.Add(item);
                        }
                    }
                }
            }

            // 4. Fetch Order Status History
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("dbo.sp_OrderStatusHistory_GetByOrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vm.StatusHistory.Add(new OrderStatusHistory
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            SubOrderId = reader["SubOrderId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["SubOrderId"]) : null,
                            Status = reader["Status"].ToString(),
                            ChangedByUserId = reader["ChangedByUserId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ChangedByUserId"]) : null,
                            ChangedByRole = reader["ChangedByRole"].ToString(),
                            Message = reader["Message"] != DBNull.Value ? reader["Message"].ToString() : null,
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }

            // 5. Build Overall 9-Step Timeline
            vm.TimelineSteps = BuildTimeline(vm);

            // 6. Build Assigned Riders Cards
            var distinctRiders = vm.SubOrders
                .Where(s => s.RiderId.HasValue && !string.IsNullOrEmpty(s.RiderName))
                .GroupBy(s => s.RiderId.Value);

            foreach (var grp in distinctRiders)
            {
                var sample = grp.First();
                var card = new RiderTrackingCard
                {
                    RiderId = grp.Key,
                    RiderName = sample.RiderName,
                    VehicleType = sample.RiderVehicle ?? "Motorbike",
                    Rating = sample.RiderRating,
                    Status = sample.DeliveryStatus,
                    ETAMinutes = sample.MinutesToDelivery,
                    AssignedRestaurants = string.Join(", ", grp.Select(s => s.RestaurantName).Distinct()),
                    CurrentLatitude = sample.RiderLatitude,
                    CurrentLongitude = sample.RiderLongitude
                };
                vm.AssignedRiders.Add(card);
            }

            // 7. Build Map Pins
            vm.DeliveryMap = new DeliveryMapViewModel
            {
                CustomerAddress = vm.DeliveryAddress,
                CustomerLatitude = 22.585000m,
                CustomerLongitude = 88.428000m,
                RemainingDistanceKm = vm.AssignedRiders.Any() ? 2.4 : 3.8,
                EstimatedMinutes = vm.OverallETAMinutes
            };

            foreach (var s in vm.SubOrders)
            {
                vm.DeliveryMap.RestaurantPins.Add(new RestaurantMapPin
                {
                    RestaurantId = s.RestaurantId,
                    RestaurantName = s.RestaurantName,
                    Latitude = s.RestaurantLatitude,
                    Longitude = s.RestaurantLongitude,
                    Status = s.PreparationStatus,
                    PickupSequence = s.PickupSequence
                });
            }

            foreach (var r in vm.AssignedRiders)
            {
                vm.DeliveryMap.RiderPins.Add(new RiderMapPin
                {
                    RiderId = r.RiderId,
                    RiderName = r.RiderName,
                    VehicleType = r.VehicleType,
                    Latitude = r.CurrentLatitude,
                    Longitude = r.CurrentLongitude,
                    Status = r.Status
                });
            }

            return vm;
        }

        public List<Order> GetCustomerOrders(int customerId)
        {
            var list = new List<Order>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("dbo.sp_OrderTracking_GetCustomerOrders", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Order
                        {
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            OrderNumber = reader["OrderNumber"].ToString(),
                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
                            CustomerName = reader["CustomerName"].ToString(),
                            TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                            DeliveryFee = Convert.ToDecimal(reader["DeliveryFee"]),
                            DeliveryAddress = reader["DeliveryAddress"] != DBNull.Value ? reader["DeliveryAddress"].ToString() : null,
                            PaymentStatus = reader["PaymentStatus"].ToString(),
                            OrderStatus = reader["OrderStatus"].ToString(),
                            DeliveryStatus = reader["DeliveryStatus"].ToString(),
                            DeliveryOtp = reader["DeliveryOtp"] != DBNull.Value ? reader["DeliveryOtp"].ToString() : null,
                            OrderedAt = Convert.ToDateTime(reader["OrderedAt"]),
                            ProcessedAt = reader["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ProcessedAt"]) : null,
                            RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : null,
                            SubOrderCount = Convert.ToInt32(reader["SubOrderCount"]),
                            RestaurantNames = reader["RestaurantNames"] != DBNull.Value ? reader["RestaurantNames"].ToString().TrimEnd(',', ' ') : "FoodReady Kitchen",
                            Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString().TrimEnd(',', ' ') : "Items"
                        });
                    }
                }
            }
            return list;
        }

        public bool SubmitRating(OrderRating rating)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_OrderRating_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", rating.OrderId);
                    cmd.Parameters.AddWithValue("@CustomerId", rating.CustomerId);
                    cmd.Parameters.AddWithValue("@SubOrderId", (object)rating.SubOrderId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FoodRating", rating.FoodRating);
                    cmd.Parameters.AddWithValue("@RestaurantRating", rating.RestaurantRating);
                    cmd.Parameters.AddWithValue("@RiderRating", rating.RiderRating);
                    cmd.Parameters.AddWithValue("@ReviewText", (object)rating.ReviewText ?? string.Empty);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool CancelOrder(int orderId, int customerId, string reason)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Order_Cancel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@Reason", reason ?? "Cancelled by customer");
                    conn.Open();
                    object res = cmd.ExecuteScalar();
                    return res != null && Convert.ToInt32(res) > 0;
                }
            }
            catch { return false; }
        }

        private List<TimelineStep> BuildTimeline(OrderTrackingViewModel vm)
        {
            var steps = new List<TimelineStep>();

            // Define the 8 canonical stages:
            // 1. Order Placed
            // 2. Payment Confirmed
            // 3. Preparing Food
            // 4. Ready for Pickup
            // 5. Rider Assigned
            // 6. Picked Up
            // 7. Out for Delivery
            // 8. Delivered

            int currentStepIndex = 1;
            string status = vm.OverallStatus;
            bool anyRiderAssigned = vm.SubOrders.Any(s => s.RiderId.HasValue);
            bool anyPickedUp = vm.SubOrders.Any(s => s.DeliveryStatus == "PickedUp" || s.DeliveryStatus == "OutForDelivery" || s.DeliveryStatus == "Delivered");
            bool anyOut = vm.SubOrders.Any(s => s.DeliveryStatus == "OutForDelivery" || s.DeliveryStatus == "Delivered");
            bool allDelivered = vm.SubOrders.Any() && vm.SubOrders.All(s => s.DeliveryStatus == "Delivered");

            if (status == "Delivered" || allDelivered) currentStepIndex = 8;
            else if (status == "OutForDelivery" || anyOut) currentStepIndex = 7;
            else if (anyPickedUp) currentStepIndex = 6;
            else if (anyRiderAssigned) currentStepIndex = 5;
            else if (vm.SubOrders.Any(s => s.PreparationStatus == "ReadyForPickup" || s.DeliveryStatus == "ReadyForPickup")) currentStepIndex = 4;
            else if (vm.SubOrders.Any(s => s.PreparationStatus == "Preparing" || s.PreparationStatus == "Confirmed") || status == "Preparing" || status == "Accepted") currentStepIndex = 3;
            else if (vm.PaymentStatus == "Success" || vm.PaymentStatus == "Paid") currentStepIndex = 2;
            else currentStepIndex = 1;

            string[] titles = new[]
            {
                "Order Placed",
                "Payment Confirmed",
                "Preparing Food",
                "Ready for Pickup",
                "Rider Assigned",
                "Picked Up from Kitchen",
                "Out for Delivery",
                "Delivered to Doorstep"
            };

            string[] descriptions = new[]
            {
                "Your order request has been received by FoodReady.",
                "Payment verified securely through transaction gateway.",
                "Chefs are preparing your meal with fresh ingredients.",
                "Packed hot & sealed with tamper-proof security tape.",
                "Delivery partners dispatched to restaurant pickup points.",
                "Rider has picked up packages from all kitchen stops.",
                "Delivery partner is speeding toward your location.",
                "Delicious food successfully delivered. Enjoy your meal!"
            };

            string[] icons = new[]
            {
                "bi-bag-check-fill",
                "bi-credit-card-2-front-fill",
                "bi-fire",
                "bi-box-seam-fill",
                "bi-person-badge-fill",
                "bi-handbag-fill",
                "bi-scooter",
                "bi-check-circle-fill"
            };

            for (int i = 0; i < 8; i++)
            {
                int stepNum = i + 1;
                string state = "Pending";
                string timeDisplay = "";

                if (stepNum < currentStepIndex)
                {
                    state = "Completed";
                    timeDisplay = vm.OrderedAt.AddMinutes(i * 3).ToString("hh:mm tt");
                }
                else if (stepNum == currentStepIndex)
                {
                    state = "Current";
                    timeDisplay = "Now";
                }
                else
                {
                    state = "Pending";
                    timeDisplay = "";
                }

                steps.Add(new TimelineStep
                {
                    StepNumber = stepNum,
                    Title = titles[i],
                    Description = descriptions[i],
                    TimeDisplay = timeDisplay,
                    State = state,
                    Icon = icons[i]
                });
            }

            return steps;
        }
    }
}
