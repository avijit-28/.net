using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestrictionOperators
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            // Func<int, bool> predicate = i => i % 2 == 0;
            // IEnumerable<int> evenNumbers = numbers.Where(predicate);
            IEnumerable<int> evenNumbers = numbers.Where(num => num % 2 == 0);
            foreach (int evenNumber in evenNumbers)
            {
                Console.WriteLine(evenNumber);
            }

            // example 1
            Console.WriteLine("Example 1: Sort Students by Name in ascending order");
            IEnumerable<Student> result1 = Student.GetAllStudents().OrderBy(s => s.Name);
            foreach (Student student in result1)
            {
                Console.WriteLine(student.Name);
            }


            // example 2
            Console.WriteLine("Example 2: Rewrite Example 1 using SQL like syntax");
            IEnumerable<Student> result2 = from student in Student.GetAllStudents()
                                          orderby student.Name
                                          select student;
            foreach (Student student in result2)
            {
                Console.WriteLine(student.Name);
            }

            //IEnumerable<Department> departments = context.Departments
            //.Where(dept => dept.Name == "IT" || dept.Name == "HR");
            //foreach (Department department in departments)
            //{
            //    Console.WriteLine("Department Name = " + department.Name);
            //    foreach (Employee employee in department
            //    .Employees.Where(emp => emp.Gender == "Male"))
            //    {
            //        Console.WriteLine("\tEmployee Name = " + employee.FirstName
            //        + " " + employee.LastName);
            //    }
            //    Console.WriteLine();
            //}
        }
    }
}
