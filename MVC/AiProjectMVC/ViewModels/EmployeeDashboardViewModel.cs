using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AiProjectMVC.Models;

namespace AiProjectMVC.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        public EmployeeUser CurrentUser { get; set; }
        public Employee EmployeeProfile { get; set; }
        public Address AddressDetails { get; set; }
        public List<Employee> DepartmentColleagues { get; set; }
        public List<EmployeeUser> SystemUsers { get; set; }

        public int TotalEmployees { get; set; }
        public int TotalAddresses { get; set; }
        public int TotalUsers { get; set; }
        public double AverageSalary { get; set; }

        public EmployeeDashboardViewModel()
        {
            DepartmentColleagues = new List<Employee>();
            SystemUsers = new List<EmployeeUser>();
        }
    }
}
