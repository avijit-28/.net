using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class PasswordResetRequest
    {
        public int RequestId { get; set; }

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Registered Email Address")]
        public string Email { get; set; }

        [Display(Name = "User Role")]
        public string UserRole { get; set; } = "Customer";

        public int? UserId { get; set; }

        [Display(Name = "Account Name")]
        public string UserName { get; set; }

        [Required, StringLength(500, MinimumLength = 5)]
        [Display(Name = "Reason for Password Reset")]
        public string Reason { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        [Display(Name = "Admin Notes")]
        public string AdminNotes { get; set; }

        public int? ApprovedByAdminId { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedByAdminName { get; set; }

        [Display(Name = "Approved At")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "Completed At")]
        public DateTime? CompletedAt { get; set; }

        [Display(Name = "Submitted At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class UserVerificationResult
    {
        public bool Found { get; set; }
        public string UserRole { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class PasswordRecoveryViewModel
    {
        [Required(ErrorMessage = "Please enter your registered email address"), EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Registered Email Address")]
        public string Email { get; set; }

        [Display(Name = "Reason for Reset / Recovery")]
        public string Reason { get; set; }

        [Display(Name = "New Password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", 
            ErrorMessage = "Password must be alphanumeric with at least one special symbol (@$!%*#?&)")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        public string ActiveTab { get; set; } = "submit"; // submit, check, reset
        public PasswordResetRequest CurrentRequest { get; set; }
        public string StatusMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
