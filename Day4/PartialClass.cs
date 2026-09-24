using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day4
{
    public partial class PartialClass
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public double Salary { get; set; }

        public bool isPresent { get; set; }

        public void EmployeeActive(bool isPresent)
        {
            if (isPresent is true)
            {
                Console.WriteLine("Active");
            }
            else { Console.WriteLine("Inactive"); }
        }
    }
}
