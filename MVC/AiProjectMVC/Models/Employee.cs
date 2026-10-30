using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiProjectMVC.Models
{
    public class Employee
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0")]
        [Display(Name = "Employee ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Salary must be a positive number greater than 0.")]
        [Display(Name = "Salary (\u20B9)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Salary { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(100, ErrorMessage = "Department cannot be longer than 100 characters.")]
        [Display(Name = "Department")]
        public string Dept { get; set; }

        [Required(ErrorMessage = "Linked Address is required. Please select an address matching the employee city.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid linked address.")]
        [Display(Name = "Linked Address")]
        public int? AddressId { get; set; }
    }
}
