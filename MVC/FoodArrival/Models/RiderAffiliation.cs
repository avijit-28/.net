using System;

namespace FoodArrival.Models
{
    public class RiderAffiliation
    {
        public int AffiliationId { get; set; }
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleType { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseDocumentUrl { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public bool IsVerified { get; set; }
        public string VerifiedByRole { get; set; }
        public int? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime AffiliatedAt { get; set; } = DateTime.Now;
    }
}
