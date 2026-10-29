using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FirstMVCProject.Models;

namespace FirstMVCProject.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }
        public Address Address { get; set; }
        public string PageTitle { get; set; }
        public string PageHeader { get; set; }
        public List<Employee> Employees { get; set; }

        public List<Address> Addresses { get; set; }
    }
}