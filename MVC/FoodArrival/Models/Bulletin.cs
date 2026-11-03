using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class Bulletin
    {
        public int BulletinId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [StringLength(50)]
        public string TargetAudience { get; set; } = "All"; // All, Customers, Restaurants, Riders

        [StringLength(50)]
        public string Priority { get; set; } = "Normal"; // Normal, Important, Urgent

        public int CreatedByAdminId { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
