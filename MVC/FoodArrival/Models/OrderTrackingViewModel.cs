using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodArrival.Models
{
    public class OrderTrackingViewModel
    {
        // 1. Order Header
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } // #FR100125
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime OrderedAt { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public string OverallStatus { get; set; } // Confirmed, Preparing, ReadyForPickup, OutForDelivery, Delivered, Cancelled
        public string DeliveryStatus { get; set; }
        public string DeliveryOtp { get; set; } // 6-digit handover OTP
        public string PaymentStatus { get; set; } // Paid, Pending, Failed
        public string PaymentMethod { get; set; } // UPI, Card, NetBanking, COD
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; } = 40.00m;
        public decimal PlatformFee { get; set; } = 15.00m;
        public decimal TotalAmount { get; set; }

        // Delay Detection
        public bool IsDelayed => EstimatedDeliveryTime.HasValue && DateTime.Now > EstimatedDeliveryTime.Value && OverallStatus != "Delivered" && OverallStatus != "Cancelled";
        public int DelayMinutes => IsDelayed ? (int)(DateTime.Now - EstimatedDeliveryTime.Value).TotalMinutes : 0;
        public string DelayReason => IsDelayed ? "High traffic & kitchen rush along the combined route" : null;

        // Overall ETA
        public int OverallETAMinutes
        {
            get
            {
                if (OverallStatus == "Delivered") return 0;
                if (EstimatedDeliveryTime.HasValue)
                {
                    int diff = (int)(EstimatedDeliveryTime.Value - DateTime.Now).TotalMinutes;
                    return diff > 0 ? diff : 5;
                }
                return 25;
            }
        }

        // 2. Order Summary Items
        public List<OrderDetail> OrderItems { get; set; } = new List<OrderDetail>();

        // 3. Overall Order Timeline Events
        public List<TimelineStep> TimelineSteps { get; set; } = new List<TimelineStep>();

        // 4. Multi-Restaurant Sub-Orders
        public List<OrderSubOrder> SubOrders { get; set; } = new List<OrderSubOrder>();

        // 5. Assigned Riders
        public List<RiderTrackingCard> AssignedRiders { get; set; } = new List<RiderTrackingCard>();

        // 6. Live Delivery Route
        public DeliveryMapViewModel DeliveryMap { get; set; } = new DeliveryMapViewModel();

        // 7. Order Status History log
        public List<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();

        // Customer permissions
        public bool CanCancel => OverallStatus == "Pending" || (OverallStatus == "Confirmed" && (DateTime.Now - OrderedAt).TotalMinutes <= 2);
        public bool IsDelivered => OverallStatus == "Delivered" || DeliveryStatus == "Delivered";
    }

    public class TimelineStep
    {
        public int StepNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeDisplay { get; set; }
        public string State { get; set; } // Completed, Current, Pending
        public string Icon { get; set; }
    }

    public class RiderTrackingCard
    {
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string VehicleType { get; set; }
        public decimal Rating { get; set; } = 4.8m;
        public string Status { get; set; }
        public int ETAMinutes { get; set; } = 15;
        public string AssignedRestaurants { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }
    }

    public class DeliveryMapViewModel
    {
        public string CustomerAddress { get; set; }
        public decimal CustomerLatitude { get; set; } = 22.585000m;
        public decimal CustomerLongitude { get; set; } = 88.428000m;

        public List<RestaurantMapPin> RestaurantPins { get; set; } = new List<RestaurantMapPin>();
        public List<RiderMapPin> RiderPins { get; set; } = new List<RiderMapPin>();

        public double RemainingDistanceKm { get; set; } = 2.4;
        public int EstimatedMinutes { get; set; } = 12;
    }

    public class RestaurantMapPin
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Status { get; set; }
        public int PickupSequence { get; set; }
    }

    public class RiderMapPin
    {
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string VehicleType { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Status { get; set; }
    }
}
