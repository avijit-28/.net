using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class Rider
    {
        public int RiderId { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Rider Name")]
        public string RiderName { get; set; }

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string VehicleType { get; set; } = "Motorbike";

        public string CurrentStatus { get; set; } = "Available"; // Available, OnDelivery, Offline

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
