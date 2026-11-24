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
        
        public virtual void DisplayEmployeeInfo()
        {
            Console.WriteLine($"Employee ID: {EmployeeId}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Base Salary: {BaseSalary}");
        }


    }

    class FullTimeEmployee : employee
    {
        public double AnnualBonus { get; set; }
        public double MedicalAllowance { get; set; }
        
        public FullTimeEmployee(int id, string name, double baseSalary, double annualBonus, double medicalAllowance)
            : base(id, name, baseSalary)
        {
            AnnualBonus = annualBonus;
            MedicalAllowance = medicalAllowance;
        }

        public double CalculateNetSalary()
        {
            return BaseSalary + AnnualBonus + MedicalAllowance;
        }

        public override void DisplayEmployeeInfo()
        {
            base.DisplayEmployeeInfo();
            Console.WriteLine($"Annual Bonus: {AnnualBonus}");
            Console.WriteLine($"Medical Allowance: {MedicalAllowance}");
            Console.WriteLine($"Net Salary: {CalculateNetSalary()}");
        }

    }
    class ContractEmployee : employee
    {
        public int ContractDurationMonths { get; set; }
        public double OvertimePay { get; set; }
        public ContractEmployee(int id, string name, double salary, int contractDuration, double overtimePay)
            : base(id, name, salary)
        {
            ContractDurationMonths = contractDuration;
            OvertimePay = overtimePay;
        }
        public double CalculateNetSalary()
        {
            return BaseSalary + OvertimePay;
        }
        public override void DisplayEmployeeInfo()
        {
            base.DisplayEmployeeInfo();
            Console.WriteLine($"Contract Duration (Months): {ContractDurationMonths}");
            Console.WriteLine($"Overtime Pay: {OvertimePay}");
            Console.WriteLine($"Net Salary: {CalculateNetSalary()}");
        }
    }

    class FreelanceEmployee : employee
    {
        public int CompletedProjects { get; set; }
        public double projectRate { get; set; }

        public FreelanceEmployee(int id, string name, double salary, double projectRate, int completedProjects)
            : base(id, name, salary )
        {
            this.projectRate = projectRate;
            this.CompletedProjects = completedProjects;
        }
        public double CalculateNetSalary()
        {
            return BaseSalary + (projectRate * CompletedProjects);
        }
        public override void DisplayEmployeeInfo()
        {
            base.DisplayEmployeeInfo();
            Console.WriteLine($"Project Rate: {projectRate}");
            Console.WriteLine($"Completed Projects: {CompletedProjects}");
            Console.WriteLine($"Net Salary: {CalculateNetSalary()}");
        }
    }
}   