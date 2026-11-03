using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        [Required(ErrorMessage = "Admin Name is required")]
        [StringLength(150)]
        [Display(Name = "Admin Name")]
        public string AdminName { get; set; }

        [StringLength(100)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
