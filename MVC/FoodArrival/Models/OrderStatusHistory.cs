using System;

namespace FoodArrival.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? SubOrderId { get; set; }
        public string Status { get; set; }
        public int? ChangedByUserId { get; set; }
        public string ChangedByRole { get; set; } = "System"; // Customer, Restaurant, Staff, Rider, Admin, System
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
