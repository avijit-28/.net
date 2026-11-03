using System;
using System.Collections.Generic;

namespace FoodArrival.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Pending";
        public DateTime OrderedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int? ProcessedBy { get; set; }
        public string RejectionReason { get; set; }
        public string Items { get; set; }

        // Multi-role workflow additions
        public int? RestaurantId { get; set; } = 1;
        public string RestaurantName { get; set; }
        public int? RiderId { get; set; }
        public string RiderName { get; set; }
        public string DeliveryStatus { get; set; } = "Pending"; // Pending, Accepted, Preparing, ReadyForPickup, OutForDelivery, Delivered

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
