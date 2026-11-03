using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodArrival.Models
{
    public class CartItem
    {
        public int FoodItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public int RestaurantId { get; set; } = 1;
        public string RestaurantName { get; set; } = "FoodReady Kitchen";
        public string Category { get; set; } = "Snacks";
        public decimal SubTotal => UnitPrice * Quantity;
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Subtotal => Items.Sum(i => i.SubTotal);
        public decimal DeliveryFee => Items.Any() ? 40.00m : 0.00m;
        public decimal PlatformFee => Items.Any() ? 15.00m : 0.00m;
        public decimal TotalAmount => Subtotal + DeliveryFee + PlatformFee;
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; } = "UPI";

        public int UniqueRestaurantCount => Items.Select(i => i.RestaurantId).Distinct().Count();
        public IEnumerable<IGrouping<string, CartItem>> GroupedByRestaurant =>
            Items.GroupBy(i => !string.IsNullOrEmpty(i.RestaurantName) ? i.RestaurantName : "Restaurant #" + i.RestaurantId);
    }

    public class AdminDashboardViewModel
    {
        public int PendingOrders { get; set; }
        public int AcceptedOrders { get; set; }
        public int RejectedOrders { get; set; }
        public int AvailableFoodItems { get; set; }
        public int ActiveCustomers { get; set; }
        public int TotalRestaurants { get; set; }
        public int TotalRiders { get; set; }
        public int TotalStaff { get; set; }
        public int PendingApprovalsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PlatformEarnings { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<Transaction> RecentTransactions { get; set; } = new List<Transaction>();
        public List<Bulletin> ActiveBulletins { get; set; } = new List<Bulletin>();
        public List<Restaurant> PendingRestaurants { get; set; } = new List<Restaurant>();
        public List<RestaurantStaff> PendingStaff { get; set; } = new List<RestaurantStaff>();
        public List<RiderAffiliation> PendingRiders { get; set; } = new List<RiderAffiliation>();
        public List<FoodItem> PendingDishes { get; set; } = new List<FoodItem>();
    }
}
