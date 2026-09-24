using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    internal class employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public double BaseSalary { get; set; }


        public employee(int id, string name, double salary)
        {
            EmployeeId = id;
            Name = name;
            BaseSalary = salary;
        }

        public double CalculateSalary()
        {
            return BaseSalary;
        }
        
        public void DisplayEmployeeInfo()
        {
            Console.WriteLine($"Employee ID: {EmployeeId}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Base Salary: {BaseSalary}");
        }


    }

    class FullTimeEmployee : employee
    {
        public double Benefits { get; set; }

        public FullTimeEmployee(int id, string name, double salary, double benefits)
            : base(id, name, salary)
        {
            Benefits = benefits;
        }
        public double CalculateNetSalary()
        {
            return BaseSalary + Benefits;
        }
    }
    class ContractEmployee : employee
    {
        public ContractEmployee(int id, string name, double salary)
            : base(id, name, salary)
        {

        }
        public double CalculateNetSalary()
        {
            return BaseSalary;
        }
    }

    class FreelanceEmployee : employee
    {
       public double projectRate { get; set; }

        public FreelanceEmployee(int id, string name, double salary, double projectRate)
            : base(id, name, salary)
        {
            this.projectRate = projectRate;
        }
        public double CalculateNetSalary()
        {
            return BaseSalary + projectRate;
        }
    }
}   