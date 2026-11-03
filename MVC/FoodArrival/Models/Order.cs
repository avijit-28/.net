using System;
using System.Collections.Generic;

namespace FoodArrival.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } // e.g. FR100125
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeliveryFee { get; set; } = 40.00m;
        public string PaymentStatus { get; set; } = "Success"; // Success, Pending, Failed
        public string PaymentMethod { get; set; } = "UPI"; // UPI, Card, NetBanking, COD
        public string OrderStatus { get; set; } = "Pending";
        public DateTime OrderedAt { get; set; } = DateTime.Now;
        public DateTime? ProcessedAt { get; set; }
        public int? ProcessedBy { get; set; }
        public string RejectionReason { get; set; }
        public string Items { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }

        // Multi-restaurant workflow additions
        public int? RestaurantId { get; set; } = 1;
        public string RestaurantName { get; set; }
        public string RestaurantNames { get; set; }
        public int? RiderId { get; set; }
        public string RiderName { get; set; }
        public string DeliveryStatus { get; set; } = "Pending"; // Pending, Accepted, Preparing, ReadyForPickup, OutForDelivery, Delivered
        public int SubOrderCount { get; set; } = 1;

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public List<OrderSubOrder> SubOrders { get; set; } = new List<OrderSubOrder>();
        public List<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}
