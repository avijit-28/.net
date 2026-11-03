using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Services
{
    public class RiderAllocationService : IRiderAllocationService
    {
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        // Configurable Scoring Weights (Configurable rather than hard-coded)
        public double WeightDistance { get; set; } = 0.25;
        public double WeightETA { get; set; } = 0.25;
        public double WeightWorkload { get; set; } = 0.15;
        public double WeightRouteCompatibility { get; set; } = 0.15;
        public double WeightAffiliation { get; set; } = 0.10;
        public double WeightVehicle { get; set; } = 0.10;

        // Configurable SLA thresholds
        public int MaxDeliverySLAMinutes { get; set; } = 45;
        public double MaxBatchPickupDistanceKm { get; set; } = 6.0;
        public int MaxPrepTimeGapMinutes { get; set; } = 15;

        // ==========================================
        // 1. ALLOCATE RIDERS (TWO-STAGE OPTIMIZATION)
        // ==========================================
        public AllocationResult AllocateRiders(int masterOrderId)
        {
            var result = new AllocationResult { MasterOrderId = masterOrderId };

            // Step 1: Load master order and restaurant sub-orders
            Order masterOrder = GetMasterOrder(masterOrderId);
            if (masterOrder == null)
            {
                result.IsSuccessful = false;
                result.Message = "Master order not found.";
                return result;
            }
            result.OrderNumber = masterOrder.OrderNumber;

            List<OrderSubOrder> subOrders = GetSubOrders(masterOrderId);
            if (!subOrders.Any())
            {
                result.IsSuccessful = false;
                result.Message = "No restaurant sub-orders found for allocation.";
                return result;
            }

            // Customer location (Default to Kolkata Sector 5 coordinates if customer has no GPS)
            decimal customerLat = 22.585000m;
            decimal customerLng = 88.428000m;

            // Step 2: Fetch eligible active riders
            List<RiderCandidateDto> eligibleRiders = FindEligibleRiders();
            if (!eligibleRiders.Any())
            {
                result.IsSuccessful = false;
                result.Message = "No delivery partners currently active or available. Queued for next dispatch cycle.";
                return result;
            }

            // Step 3: Two-Stage Optimization
            // Stage 1: Try to create the minimum feasible number of rider routes
            // Check if ALL sub-orders can be batched to a SINGLE rider
            var singleRiderCandidates = new List<Tuple<RiderCandidateDto, double, DateTime, double>>();
            foreach (var rider in eligibleRiders)
            {
                if (CanBatchRestaurants(rider, subOrders, customerLat, customerLng))
                {
                    double dist = CalculateTotalRouteDistance(rider, subOrders, customerLat, customerLng);
                    DateTime eta = CalculateEstimatedDeliveryTime(subOrders, rider, customerLat, customerLng);
                    double score = CalculateRouteScore(rider, subOrders, customerLat, customerLng);
                    singleRiderCandidates.Add(Tuple.Create(rider, score, eta, dist));
                }
            }

            // If a single rider can feasible fulfill all restaurants within SLA & capacity:
            if (singleRiderCandidates.Any())
            {
                // Select candidate with the best composite score (Stage 2)
                var bestCandidate = singleRiderCandidates.OrderByDescending(x => x.Item2).First();
                var rider = bestCandidate.Item1;
                var eta = bestCandidate.Item3;
                var totalDist = bestCandidate.Item4;

                var route = new AllocationRouteDto
                {
                    RiderId = rider.RiderId,
                    RiderName = rider.RiderName,
                    VehicleType = rider.VehicleType,
                    EstimatedDeliveryTime = eta,
                    TotalDistanceKm = Math.Round(totalDist, 2),
                    RouteScore = Math.Round(bestCandidate.Item2, 2),
                    SLAStatus = (eta - DateTime.Now).TotalMinutes <= MaxDeliverySLAMinutes ? "On Time" : "At Risk"
                };

                // Assign pickup sequences
                int seq = 1;
                foreach (var sub in subOrders.OrderBy(s => s.MinutesToReady))
                {
                    AssignRider(sub.SubOrderId, rider.RiderId, seq, eta);
                    route.Restaurants.Add(new RouteRestaurantStopDto
                    {
                        RestaurantId = sub.RestaurantId,
                        RestaurantName = sub.RestaurantName,
                        SubOrderId = sub.SubOrderId,
                        PickupSequence = seq,
                        EstimatedPickupTime = DateTime.Now.AddMinutes(12 + (seq * 3)),
                        EstimatedReadyTime = sub.EstimatedReadyAt ?? DateTime.Now.AddMinutes(15)
                    });
                    seq++;
                }

                result.Routes.Add(route);
                result.IsSuccessful = true;
                result.Message = $"Intelligently optimized: 1 rider assigned for all {subOrders.Count} restaurants ({rider.RiderName}).";
            }
            else
            {
                // Multi-Rider Split: Group or allocate 1 rider per restaurant sub-order
                var usedRiderIds = new HashSet<int>();
                int routeIndex = 1;

                foreach (var sub in subOrders)
                {
                    var singleSubList = new List<OrderSubOrder> { sub };
                    // Find best candidate for this restaurant
                    var candidatesForSub = eligibleRiders
                        .Where(r => !usedRiderIds.Contains(r.RiderId) && r.ApprovedRestaurantIds.Contains(sub.RestaurantId))
                        .Select(r => new
                        {
                            Rider = r,
                            Score = CalculateRouteScore(r, singleSubList, customerLat, customerLng),
                            ETA = CalculateEstimatedDeliveryTime(singleSubList, r, customerLat, customerLng),
                            Dist = CalculateTotalRouteDistance(r, singleSubList, customerLat, customerLng)
                        })
                        .OrderByDescending(x => x.Score)
                        .FirstOrDefault();

                    // Fallback to any eligible rider if restaurant-specific is busy
                    if (candidatesForSub == null)
                    {
                        candidatesForSub = eligibleRiders
                            .Where(r => !usedRiderIds.Contains(r.RiderId))
                            .Select(r => new
                            {
                                Rider = r,
                                Score = CalculateRouteScore(r, singleSubList, customerLat, customerLng),
                                ETA = CalculateEstimatedDeliveryTime(singleSubList, r, customerLat, customerLng),
                                Dist = CalculateTotalRouteDistance(r, singleSubList, customerLat, customerLng)
                            })
                            .OrderByDescending(x => x.Score)
                            .FirstOrDefault();
                    }

                    if (candidatesForSub != null)
                    {
                        var rider = candidatesForSub.Rider;
                        usedRiderIds.Add(rider.RiderId);

                        AssignRider(sub.SubOrderId, rider.RiderId, 1, candidatesForSub.ETA);

                        var route = new AllocationRouteDto
                        {
                            RiderId = rider.RiderId,
                            RiderName = rider.RiderName,
                            VehicleType = rider.VehicleType,
                            EstimatedDeliveryTime = candidatesForSub.ETA,
                            TotalDistanceKm = Math.Round(candidatesForSub.Dist, 2),
                            RouteScore = Math.Round(candidatesForSub.Score, 2),
                            SLAStatus = "On Time"
                        };
                        route.Restaurants.Add(new RouteRestaurantStopDto
                        {
                            RestaurantId = sub.RestaurantId,
                            RestaurantName = sub.RestaurantName,
                            SubOrderId = sub.SubOrderId,
                            PickupSequence = 1,
                            EstimatedPickupTime = DateTime.Now.AddMinutes(15),
                            EstimatedReadyTime = sub.EstimatedReadyAt ?? DateTime.Now.AddMinutes(15)
                        });

                        result.Routes.Add(route);
                    }
                    routeIndex++;
                }

                result.IsSuccessful = result.Routes.Any();
                result.Message = $"Multi-rider allocation complete: {result.Routes.Count} riders deployed to safeguard delivery SLA.";
            }

            // Step 4: Synchronize Master Order status
            UpdateMasterOrderStatus(masterOrderId, "Confirmed", "RiderAssigned");

            // Step 5: Send Tailored Notifications
            SendAllocationNotifications(masterOrder, subOrders, result);

            return result;
        }

        // ==========================================
        // 2. FIND ELIGIBLE RIDERS
        // ==========================================
        public List<RiderCandidateDto> FindEligibleRiders()
        {
            var list = new List<RiderCandidateDto>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetEligibleForAllocation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var candidate = new RiderCandidateDto
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                CurrentStatus = reader["CurrentStatus"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                MaxCapacity = Convert.ToInt32(reader["MaxCapacity"]),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                Latitude = Convert.ToDecimal(reader["Latitude"]),
                                Longitude = Convert.ToDecimal(reader["Longitude"]),
                                CurrentWorkload = Convert.ToInt32(reader["CurrentWorkload"])
                            };

                            string rawIds = reader["ApprovedRestaurantIds"] != DBNull.Value ? reader["ApprovedRestaurantIds"].ToString() : string.Empty;
                            if (!string.IsNullOrEmpty(rawIds))
                            {
                                foreach (var part in rawIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int id;
                                    if (int.TryParse(part.Trim(), out id))
                                    {
                                        candidate.ApprovedRestaurantIds.Add(id);
                                    }
                                }
                            }
                            list.Add(candidate);
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        // ==========================================
        // 3. CAN BATCH RESTAURANTS CHECK
        // ==========================================
        public bool CanBatchRestaurants(RiderCandidateDto rider, List<OrderSubOrder> subOrders, decimal customerLat, decimal customerLng)
        {
            if (rider == null || !rider.IsActive || subOrders == null || !subOrders.Any())
                return false;

            // 1. Capacity constraint
            if (subOrders.Count > rider.MaxCapacity)
                return false;

            // 2. Restaurant Affiliation check
            foreach (var sub in subOrders)
            {
                if (!rider.ApprovedRestaurantIds.Contains(sub.RestaurantId))
                    return false;
            }

            // 3. Preparation Time Synchronization check
            // If ready times differ by more than MaxPrepTimeGapMinutes, batching would make earlier food get cold
            if (subOrders.Count > 1)
            {
                var readyTimes = subOrders.Select(s => s.EstimatedReadyAt ?? DateTime.Now.AddMinutes(15)).ToList();
                var minReady = readyTimes.Min();
                var maxReady = readyTimes.Max();
                if ((maxReady - minReady).TotalMinutes > MaxPrepTimeGapMinutes)
                    return false;
            }

            // 4. Geographic compatibility check
            double totalDist = CalculateTotalRouteDistance(rider, subOrders, customerLat, customerLng);
            if (totalDist > MaxBatchPickupDistanceKm)
                return false;

            // 5. Delivery SLA check (Total time from order must be within SLA)
            DateTime eta = CalculateEstimatedDeliveryTime(subOrders, rider, customerLat, customerLng);
            if ((eta - DateTime.Now).TotalMinutes > MaxDeliverySLAMinutes)
                return false;

            return true;
        }

        // ==========================================
        // 4. CALCULATE ROUTE SCORE
        // ==========================================
        public double CalculateRouteScore(RiderCandidateDto rider, List<OrderSubOrder> subOrders, decimal customerLat, decimal customerLng)
        {
            if (rider == null) return 0;

            // Distance component (closer is higher score: 0 to 100)
            double distanceKm = CalculateTotalRouteDistance(rider, subOrders, customerLat, customerLng);
            double distanceScore = Math.Max(0, 100 - (distanceKm * 10));

            // ETA component (shorter ETA is higher score: 0 to 100)
            DateTime eta = CalculateEstimatedDeliveryTime(subOrders, rider, customerLat, customerLng);
            double etaMinutes = (eta - DateTime.Now).TotalMinutes;
            double etaScore = Math.Max(0, 100 - (etaMinutes * 2));

            // Workload component (lower current active deliveries is higher score)
            double workloadScore = Math.Max(0, 100 - (rider.CurrentWorkload * 30));

            // Route compatibility score
            double routeCompatScore = subOrders.Count > 1 ? (100 - (distanceKm * 8)) : 90;

            // Affiliation score
            int matchCount = subOrders.Count(s => rider.ApprovedRestaurantIds.Contains(s.RestaurantId));
            double affiliationScore = ((double)matchCount / Math.Max(1, subOrders.Count)) * 100;

            // Vehicle suitability score (Motorcycle/Bike higher for longer dist, Bicycle good for short dist)
            double vehicleScore = (rider.VehicleType.Contains("Bike") || rider.VehicleType.Contains("Motorcycle")) ? 95 : 75;

            // Composite weighted formula
            double totalScore = (distanceScore * WeightDistance) +
                                (etaScore * WeightETA) +
                                (workloadScore * WeightWorkload) +
                                (routeCompatScore * WeightRouteCompatibility) +
                                (affiliationScore * WeightAffiliation) +
                                (vehicleScore * WeightVehicle);

            return Math.Round(totalScore, 2);
        }

        // ==========================================
        // 5. CALCULATE ESTIMATED DELIVERY TIME
        // ==========================================
        public DateTime CalculateEstimatedDeliveryTime(List<OrderSubOrder> subOrders, RiderCandidateDto rider, decimal customerLat, decimal customerLng)
        {
            double distKm = CalculateTotalRouteDistance(rider, subOrders, customerLat, customerLng);

            // Travel speed: Motorcycle avg 25 km/h, Bicycle avg 15 km/h
            double speedKmH = (rider.VehicleType == "Bicycle") ? 15.0 : 25.0;
            double travelMinutes = (distKm / speedKmH) * 60.0;

            // Max kitchen ready time among the sub-orders
            int maxPrepMinutes = subOrders.Any() ? subOrders.Max(s => s.MinutesToReady) : 15;

            // Stop handoff buffer (3 mins per restaurant pickup)
            int stopBuffer = subOrders.Count * 3;

            int totalMinutes = (int)Math.Ceiling(Math.Max(maxPrepMinutes, travelMinutes) + stopBuffer + 5);
            return DateTime.Now.AddMinutes(totalMinutes);
        }

        // ==========================================
        // 6. ASSIGN & REASSIGN RIDERS
        // ==========================================
        public bool AssignRider(int subOrderId, int riderId, int pickupSequence, DateTime estimatedDeliveryAt)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_OrderSubOrder_AssignRider", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubOrderId", subOrderId);
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@PickupSequence", pickupSequence);
                    cmd.Parameters.AddWithValue("@EstimatedDeliveryAt", estimatedDeliveryAt);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public bool ReassignRider(int subOrderId, int newRiderId, string reason)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        UPDATE dbo.OrderSubOrders
                        SET RiderId = @NewRiderId,
                            DeliveryStatus = 'RiderAssigned'
                        WHERE SubOrderId = @SubOrderId";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewRiderId", newRiderId);
                        cmd.Parameters.AddWithValue("@SubOrderId", subOrderId);
                        cmd.ExecuteNonQuery();
                    }

                    // Add history event
                    string histSql = @"
                        INSERT INTO dbo.OrderStatusHistory (OrderId, SubOrderId, Status, ChangedByRole, Message)
                        SELECT OrderId, @SubOrderId, 'RiderReassigned', 'Admin', 'Rider reassigned: ' + @Reason
                        FROM dbo.OrderSubOrders WHERE SubOrderId = @SubOrderId";
                    using (var cmdHist = new SqlCommand(histSql, conn))
                    {
                        cmdHist.Parameters.AddWithValue("@SubOrderId", subOrderId);
                        cmdHist.Parameters.AddWithValue("@Reason", reason ?? "Operational rebalance");
                        cmdHist.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch { return false; }
        }

        public AllocationResult RecalculateAssignments(int masterOrderId)
        {
            return AllocateRiders(masterOrderId);
        }

        // ==========================================
        // HELPER METHODS
        // ==========================================
        private double CalculateTotalRouteDistance(RiderCandidateDto rider, List<OrderSubOrder> subOrders, decimal customerLat, decimal customerLng)
        {
            double totalKm = 0;
            if (!subOrders.Any()) return 0;

            // Distance from Rider current location to first restaurant
            var firstRest = subOrders.First();
            totalKm += HaversineDistance((double)rider.Latitude, (double)rider.Longitude,
                                         (double)firstRest.RestaurantLatitude, (double)firstRest.RestaurantLongitude);

            // Inter-restaurant distances
            for (int i = 0; i < subOrders.Count - 1; i++)
            {
                totalKm += HaversineDistance((double)subOrders[i].RestaurantLatitude, (double)subOrders[i].RestaurantLongitude,
                                             (double)subOrders[i + 1].RestaurantLatitude, (double)subOrders[i + 1].RestaurantLongitude);
            }

            // Distance from last restaurant to customer
            var lastRest = subOrders.Last();
            totalKm += HaversineDistance((double)lastRest.RestaurantLatitude, (double)lastRest.RestaurantLongitude,
                                         (double)customerLat, (double)customerLng);

            return totalKm;
        }

        private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Earth radius in km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * (Math.PI / 180.0);

        private Order GetMasterOrder(int orderId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("SELECT OrderId, OrderNumber, CustomerId, TotalAmount, OrderStatus, OrderedAt FROM dbo.Orders WHERE OrderId = @OrderId", conn))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                OrderNumber = reader["OrderNumber"] != DBNull.Value ? reader["OrderNumber"].ToString() : "FR" + (100000 + orderId),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                OrderedAt = Convert.ToDateTime(reader["OrderedAt"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private List<OrderSubOrder> GetSubOrders(int orderId)
        {
            var list = new List<OrderSubOrder>();
            try
            {
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
                            list.Add(new OrderSubOrder
                            {
                                SubOrderId = Convert.ToInt32(reader["SubOrderId"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RestaurantLatitude = Convert.ToDecimal(reader["RestaurantLatitude"]),
                                RestaurantLongitude = Convert.ToDecimal(reader["RestaurantLongitude"]),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                PreparationStatus = reader["PreparationStatus"].ToString(),
                                EstimatedReadyAt = reader["EstimatedReadyAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["EstimatedReadyAt"]) : null,
                                RiderId = reader["RiderId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["RiderId"]) : null,
                                DeliveryStatus = reader["DeliveryStatus"].ToString()
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        private void UpdateMasterOrderStatus(int masterOrderId, string orderStatus, string deliveryStatus)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.Orders SET OrderStatus = @OrderStatus, DeliveryStatus = @DeliveryStatus WHERE OrderId = @OrderId", conn))
                {
                    cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
                    cmd.Parameters.AddWithValue("@DeliveryStatus", deliveryStatus);
                    cmd.Parameters.AddWithValue("@OrderId", masterOrderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private void SendAllocationNotifications(Order masterOrder, List<OrderSubOrder> subOrders, AllocationResult result)
        {
            try
            {
                // Customer Notification
                _notifRepo.AddNotification("Customer", masterOrder.CustomerId, "Delivery Partner Assigned",
                    $"Your order #{result.OrderNumber} has been assigned for pickup and delivery.", "Order", $"/OrderTracking/Track?orderId={masterOrder.OrderId}");

                // Restaurant Notifications (Each restaurant only receives info relevant to its own sub-order)
                foreach (var sub in subOrders)
                {
                    var assignedRoute = result.Routes.FirstOrDefault(r => r.Restaurants.Any(rest => rest.SubOrderId == sub.SubOrderId));
                    string riderTxt = assignedRoute != null ? $"Rider {assignedRoute.RiderName}" : "A delivery rider";
                    _notifRepo.AddNotification("Restaurant", sub.RestaurantId, "Rider Assigned to Sub-Order",
                        $"{riderTxt} has been assigned to pick up Order #{result.OrderNumber} sub-order.", "Order", "/Restaurant/Dashboard");
                }

                // Rider Notifications
                foreach (var route in result.Routes)
                {
                    string restNames = string.Join(" & ", route.Restaurants.Select(r => r.RestaurantName));
                    _notifRepo.AddNotification("Rider", route.RiderId, "New Pickup Assigned!",
                        $"You have been assigned to pick up Order #{result.OrderNumber} from: {restNames}.", "Order", "/Rider/Dashboard");
                }
            }
            catch { }
        }
    }
}
