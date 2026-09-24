using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodOverriding
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public double Salary { get; set; }
        
        public virtual double CalculateBonus(double Salary)
        {
            return 50000;
        }

    }
    public class Developer : Employee
    {
        public override double CalculateBonus(double Salary)
        {
            double  baseSalary = base.CalculateBonus(Salary);
            double calculatedSalary = Salary * 0.10 + Salary;
            if (calculatedSalary > baseSalary)
            {
                return calculatedSalary;
            }
            else
            {
                return baseSalary;
            }

        }
    }
    public class Manager : Employee
    {
        public override double CalculateBonus(double Salary)
        {
            double baseSalary = base.CalculateBonus(Salary);
            double calculatedSalary = Salary * 0.20 + Salary;

            if (calculatedSalary > baseSalary)
            {
                return calculatedSalary;
            }
            else
            {
                return baseSalary;
            }
        }
    }
    public class Admin : Employee
    {
      
    }
}
