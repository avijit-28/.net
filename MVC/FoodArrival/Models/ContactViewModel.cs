using System.Collections.Generic;

namespace FoodArrival.Models
{
    public class ContactViewModel
    {
        public string SenderRole { get; set; } = "Customer"; // Customer, RestaurantStaff, Rider, Restaurant, General
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPhone { get; set; }

        // Specific Restaurant Connection (for Staff / Rider)
        public int? SpecificRestaurantId { get; set; }
        public string SpecificRestaurantName { get; set; }
        public string SpecificRestaurantPhone { get; set; }
        public string SpecificRestaurantAddress { get; set; }
        public List<RiderAffiliation> RiderAffiliatedRestaurants { get; set; } = new List<RiderAffiliation>();

        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
