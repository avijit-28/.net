using System;

namespace FoodArrival.Models
{
    public class OrderRating
    {
        public int RatingId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? SubOrderId { get; set; }
        public int FoodRating { get; set; } = 5;
        public int RestaurantRating { get; set; } = 5;
        public int RiderRating { get; set; } = 5;
        public string ReviewText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
