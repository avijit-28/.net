using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Day_6
{
    public class Student
    {
        public string SId { get; set; }
        public string SName { get; set; }
        public int Semester { get; set; }
        public double Cgpa { get; set; }

        public void Display()
        {
            Console.WriteLine($"Roll No : {SId}");
            Console.WriteLine($"Name : {SName}");
            Console.WriteLine($"Semester: {Semester}");
            Console.WriteLine($"CGPA : {Cgpa}");
            Console.WriteLine("------------------------");
        }
    }
}
