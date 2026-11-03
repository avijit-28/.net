using System;
using System.Collections.Generic;

namespace FoodArrival.Models
{
    public class OrderSubOrder
    {
        public int SubOrderId { get; set; }
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhone { get; set; }
        public string Cuisine { get; set; }
        public string RestaurantImage { get; set; }
        public decimal RestaurantLatitude { get; set; }
        public decimal RestaurantLongitude { get; set; }

        public decimal SubTotal { get; set; }
        public string PreparationStatus { get; set; } = "Confirmed"; // Pending, Confirmed, Preparing, ReadyForPickup, Completed, Cancelled
        public DateTime? EstimatedReadyAt { get; set; }

        public int? RiderId { get; set; }
        public string RiderName { get; set; }
        public string RiderVehicle { get; set; }
        public decimal RiderRating { get; set; } = 4.8m;
        public decimal RiderLatitude { get; set; }
        public decimal RiderLongitude { get; set; }

        public string DeliveryStatus { get; set; } = "Pending"; // Pending, RiderAssigned, PickedUp, OutForDelivery, Delivered
        public int PickupSequence { get; set; } = 1;
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderDetail> Items { get; set; } = new List<OrderDetail>();

        // Helpers
        public bool IsDelayed => EstimatedReadyAt.HasValue && DateTime.Now > EstimatedReadyAt.Value && PreparationStatus != "ReadyForPickup" && PreparationStatus != "Completed";
        public int MinutesToReady => EstimatedReadyAt.HasValue ? Math.Max(0, (int)(EstimatedReadyAt.Value - DateTime.Now).TotalMinutes) : 15;
        public int MinutesToDelivery => EstimatedDeliveryAt.HasValue ? Math.Max(0, (int)(EstimatedDeliveryAt.Value - DateTime.Now).TotalMinutes) : 25;
    }
}
