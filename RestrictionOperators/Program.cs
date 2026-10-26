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


            IEnumerable<Student> result = Student.GetAllStudents().OrderBy(s => s.Name);
            foreach (Student student in result)
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
