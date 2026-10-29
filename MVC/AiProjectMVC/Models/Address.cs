using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiProjectMVC.Models
{
    public class Address
    {
        [Display(Name = "Address ID")]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "PIN code is required.")]
        [StringLength(20, ErrorMessage = "PIN cannot exceed 20 characters.")]
        [Display(Name = "PIN / Postal Code")]
        public string Pin { get; set; }
    }
}
