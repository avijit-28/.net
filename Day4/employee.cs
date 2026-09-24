using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day4
{
    public partial class PartialClass
    {
        public void DisplayName() 
        {
            Console.WriteLine($"Full Name : {FirstName}{LastName}"); 
        }
        public void DisplayDetails()
        {
            Console.WriteLine("Employee Details");
            Console.WriteLine($"First Name: {FirstName}");
            Console.WriteLine($"Last Name : {LastName}");
            Console.WriteLine($"Gender : {Gender}");
            Console.WriteLine($"Salary : {Salary}");
        }

    }
}
