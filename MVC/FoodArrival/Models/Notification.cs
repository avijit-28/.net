using System;

namespace FoodArrival.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string UserRole { get; set; } = "All"; // Customer, Restaurant, Rider, Admin, All
        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; } = "Info"; // Order, Promo, Alert, System
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string TargetUrl { get; set; }
    }
}
