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

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
