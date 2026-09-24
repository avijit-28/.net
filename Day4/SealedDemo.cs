using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day4
{
    public class Employee
    {
        public int Eid, Eage;
        public string Ename, Eaddress;

        public virtual void GetEmployeeDetails()
        {
            Console.WriteLine("Employee Details");
            Console.WriteLine("Employee id :");
            Eid = int.Parse(Console.ReadLine());
            Console.WriteLine("Employee Name:");
            Ename = Console.ReadLine();
            Console.WriteLine("Employee age :");
            Eage = int.Parse(Console.ReadLine());
            Console.WriteLine("Employee Address :");
            Eaddress = Console.ReadLine();

        }
        public virtual void DisplayDetails()
        {
            Console.WriteLine("\n Employee Details");
            Console.WriteLine($"Employee ID: {Eid}");
            Console.WriteLine($"Employe Name: {Ename}");
            Console.WriteLine($"Employee Age : {Eage}");
            Console.WriteLine($"Employee Address : {Eaddress}");
        }

    }

    public sealed class Manager : Employee
    {
        double Bonus, Salary;
        public override void GetEmployeeDetails()
        {
                
        }
    }
}
