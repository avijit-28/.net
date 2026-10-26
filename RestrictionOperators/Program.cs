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
            IEnumerable<Student> result1 = Student.GetAllStudents().OrderByDescending(s => s.Name);
            foreach (Student student in result1)
            {
                Console.WriteLine(student.Name);
            }


            // example 2
            //Console.WriteLine("Example 2: Rewrite Example 1 using SQL like syntax");
            //IEnumerable<Student> result2 = from student in Student.GetAllStudents()
            //                               orderby student.Name 
            //                               select student;
            //foreach (Student student in result2)
            //{
            //    Console.WriteLine(student.Name);
            //}
            //Console.WriteLine("\n");

            ////Example 1: Get Employee Count By Department
            //var employeeGroup = from employee in Employee.GetAllEmployees()
            //                    group employee by employee.Department;
            //foreach (var group in employeeGroup)
            //{
            //    Console.WriteLine("{0} -> {1}", group.Key, group.Count());
            //}
            //Console.WriteLine("***************************************");
            //Console.WriteLine("\n");

            ////Example 2: Get Employee Count By Department and also each employee and department name
            //var employeeGroup2 = from employee in Employee.GetAllEmployees()
            //        group employee by employee.Department;
            //foreach (var group in employeeGroup2)
            //{
            //    Console.WriteLine("{0} => {1}", group.Key, group.Count());
            //    Console.WriteLine("----------");
            //    foreach (var employee in group)
            //    {
            //        Console.WriteLine(employee.Name + "\t" + employee.Department);
            //    }
            //    Console.WriteLine(); Console.WriteLine();
            //}
            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++");
            ////Example 3

            //var employeeGroup3 = from employee in Employee.GetAllEmployees()
            //                     group employee by employee.Department into eGroup
            //                     orderby eGroup.Key
            //                     select new
            //                     {
            //                         Key = eGroup.Key,
            //                         Employees = eGroup.OrderBy(x => x.Name)
            //                     };
            //foreach (var group in employeeGroup3)
            //{
            //    Console.WriteLine("{0} - {1}", group.Key, group.Employees.Count());
            //    Console.WriteLine("----------");
            //    foreach (var employee in group.Employees)
            //    {
            //        Console.WriteLine(employee.Name + "\t" + employee.Department);
            //    }
            //    Console.WriteLine(); Console.WriteLine();
            //}
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++");


            var edges = new List<(string, string)>
                {
                    ("A","B"),
                    ("A","C"),
                    ("A","D"),
                    ("A","E"),
                    ("B","C"),
                    ("C","D"),
                    ("D","E")
                };

            var result3 = edges.SelectMany(e => new[] { e.Item1, e.Item2 }).GroupBy(v => v).Select(g => new { vertex = g.Key, Degrees = g.Count() });

            foreach (var item in result3)
            {
                Console.WriteLine(item.vertex + " -> " + item.Degrees);
            }

            var result = edges.SelectMany(e => new[] { e.Item1, e.Item2 }).GroupBy(v => v).Where(g => g.Count() > 3).Select(g => g.Key);

            Console.WriteLine("Degrees higher than 3");
            foreach (var vertex in result)
            {
                Console.WriteLine(vertex);
            }
            


        }
    }
}
