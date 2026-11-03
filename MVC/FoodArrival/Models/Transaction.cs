using System;

namespace FoodArrival.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int OrderId { get; set; }
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "UPI / Card";
        public string PaymentStatus { get; set; } = "Success";
        public decimal RestaurantShare { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public string CustomerName { get; set; }
        public string RestaurantName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
