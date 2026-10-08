using System;
using System.Data.SqlClient;
using System.Linq;
using GenericSpDemo.Models;
using GenericSpDemo.Services;

namespace GenericSpDemo
{
    class Program
    {
        static void Main()
        {
            var service = new StudentService();
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("===== Student Search =====");
                Console.WriteLine("1. Search students");
                Console.WriteLine("2. Show toppers (marks >= 80)");
                Console.WriteLine("3. Exit");
                Console.Write("\nEnter your choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        SearchStudents(service);
                        break;
                    case "2":
                        ShowToppers(service);
                        break;
                    case "3":
                        running = false;
                        continue;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.Write("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void SearchStudents(StudentService service)
        {
            Console.Write("\nEnter course (BCA / MCA, blank for all): ");
            string course = Console.ReadLine();

            int minMarks = ReadInt("Enter minimum marks (0-100): ", 0, 100);

            try
            {
                StudentResult result = service.GetStudents(course, minMarks);
                DisplayResult(result);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("\nDatabase error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected error: " + ex.Message);
            }
        }

        static void ShowToppers(StudentService service)
        {
            try
            {
                StudentResult result = service.GetStudents(null, 0);
                var toppers = result.Students
                                    .Where(s => s.Marks >= 80)
                                    .OrderByDescending(s => s.Marks)
                                    .ToList();

                Console.WriteLine("\n--- Toppers ---");
                if (toppers.Count == 0)
                {
                    Console.WriteLine("No toppers found.");
                    return;
                }

                foreach (var s in toppers)
                    Console.WriteLine($"{s.Name,-12} {s.Course,-6} {s.Marks}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("\nDatabase error: " + ex.Message);
            }
        }

        static void DisplayResult(StudentResult result)
        {
            if (result.Students.Count == 0)
            {
                Console.WriteLine("\nNo students found for the given filter.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{"ID",-5}{"Name",-14}{"Course",-8}{"Marks",-6}");
            Console.WriteLine(new string('-', 33));

            foreach (var s in result.Students)
                Console.WriteLine($"{s.StudentId,-5}{s.Name,-14}{s.Course,-8}{s.Marks,-6}");

            Console.WriteLine(new string('-', 33));
            Console.WriteLine($"Total students : {result.TotalStudents}");
            Console.WriteLine($"Average marks  : {result.AverageMarks:F2}");
        }

        // Keeps asking until the user enters a valid whole number in range
        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                    return value;

                Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }
    }
}