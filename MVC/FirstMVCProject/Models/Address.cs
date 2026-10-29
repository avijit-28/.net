using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FirstMVCProject.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Country must be between 2 and 100 characters")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "State must be between 2 and 100 characters")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "City must be between 2 and 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "PIN is required")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "PIN must be exactly 6 digits")]
        public string Pin { get; set; }
    }
}