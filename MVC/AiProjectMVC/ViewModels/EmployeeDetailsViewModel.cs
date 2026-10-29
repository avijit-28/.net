using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AiProjectMVC.Models;

namespace AiProjectMVC.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }
        public Address Address { get; set; }
        public string PageTitle { get; set; }
        public string PageHeader { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Address> Addresses { get; set; }

        public EmployeeDetailsViewModel()
        {
            Employees = new List<Employee>();
            Addresses = new List<Address>();
        }
    }
}
