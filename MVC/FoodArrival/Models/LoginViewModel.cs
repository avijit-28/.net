using System.ComponentModel.DataAnnotations;
using System.Web;

namespace FoodArrival.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email or Username is required")]
        [Display(Name = "Email or Username")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        [Display(Name = "Login Role")]
        public string SelectedRole { get; set; } = "Auto"; // Auto, Customer, RestaurantOwner, RestaurantStaff, Rider, Admin
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please select your role")]
        [Display(Name = "Register As (Select Role)")]
        public string Role { get; set; } = "Customer"; // Customer, RestaurantOwner, RestaurantStaff, Rider

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_@.-]+$", ErrorMessage = "Username can only contain letters, numbers, and characters (@, ., -, _)")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(150, ErrorMessage = "Full Name cannot exceed 150 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        // Alias for backward compatibility
        public string CustomerName
        {
            get => FullName;
            set => FullName = value;
        }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone Number must be exactly 10 digits")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
            ErrorMessage = "Password must contain letters, numbers, and at least one special symbol (e.g. @$!%*?&#)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        // --- Role Specific Fields ---

        // Customer
        [Display(Name = "Delivery Address")]
        [StringLength(500)]
        public string Address { get; set; }

        // Restaurant Owner
        [Display(Name = "Restaurant Name")]
        [StringLength(150)]
        public string RestaurantName { get; set; }

        [Display(Name = "Owner Full Name")]
        [StringLength(150)]
        public string OwnerName { get; set; }

        [Display(Name = "Restaurant Physical Address")]
        [StringLength(500)]
        public string RestaurantAddress { get; set; }

        [Display(Name = "Cuisine Specialization")]
        [StringLength(150)]
        public string Cuisine { get; set; }

        // Staff of Restaurant
        [Display(Name = "Select Restaurant")]
        public int? SelectedRestaurantId { get; set; }

        [Display(Name = "Role / Designation")]
        [StringLength(100)]
        public string RoleTitle { get; set; } = "Kitchen Staff";

        // Rider
        [Display(Name = "Vehicle Type")]
        [StringLength(50)]
        public string VehicleType { get; set; } = "Motorcycle / Bike";

        [Display(Name = "Do you have a Driving License?")]
        public bool HasLicense { get; set; } = true;

        [Display(Name = "Driving License Number")]
        [StringLength(100)]
        public string LicenseNumber { get; set; }

        [Display(Name = "Upload Driving License (Photo/PDF)")]
        public HttpPostedFileBase LicenseDocumentFile { get; set; }
    }
}
