using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class RestaurantStaff
    {
        public int StaffId { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Staff ID")]
        public string StaffCode { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string PasswordHash { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public int RestaurantId { get; set; }

        [Display(Name = "Restaurant")]
        public string RestaurantName { get; set; }

        [StringLength(100)]
        [Display(Name = "Role / Designation")]
        public string RoleTitle { get; set; } = "Kitchen Staff";

        public bool IsVerified { get; set; } = false;

        public string VerifiedByRole { get; set; }

        public int? VerifiedBy { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [Display(Name = "Orders Prepared")]
        public int OrdersPrepared { get; set; } = 0;

        public int OrdersPreparedCount { get { return OrdersPrepared; } set { OrdersPrepared = value; } }

        public string FullName { get { return StaffName; } set { StaffName = value; } }

        public string ShiftStatus { get; set; } = "On Duty (Active)";

        [Display(Name = "PMS Rating")]
        public decimal PerformanceRating { get; set; } = 4.8m;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
