using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }

        [StringLength(150)]
        [Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [StringLength(100)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(100)]
        public string Cuisine { get; set; }

        public decimal Rating { get; set; } = 4.5m;

        public string ImageUrl { get; set; }

        public bool IsVerified { get; set; } = true;

        public int? VerifiedBy { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public int? EstimatedPrepMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
