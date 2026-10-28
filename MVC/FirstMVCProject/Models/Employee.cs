using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FirstMVCProject.Models
{
    public class Employee
    {
        [Required(ErrorMessage = "Employee ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0")]
        public int ID { get; set; }


        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Salary is required")]
        [Range(1, 10000000,
            ErrorMessage = "Salary must be greater than 0")]
        public int Salary { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100,
           ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(100,
            ErrorMessage = "Department cannot exceed 100 characters")]
        public string Dept { get; set; }
    }
}