using System;

namespace DeparmentProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DepartmentRepo Repo = new DepartmentRepo();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("==============================================");
                Console.WriteLine("          DEPARTMENT MANAGEMENT SYSTEM        ");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("  1. View All Departments");
                Console.WriteLine("  2. View Department By ID");
                Console.WriteLine("  3. Insert Department");
                Console.WriteLine("  4. Update Department");
                Console.WriteLine("  5. Delete Department");
                Console.WriteLine("  6. Exit");
                Console.WriteLine();
                Console.WriteLine("==============================================");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("==============================================");
                        Console.WriteLine("              ALL DEPARTMENTS                 ");
                        Console.WriteLine("==============================================");
                        Console.WriteLine();

                        Repo.getallDepartmentByProcedure();

                        break;

                    case "2":
                        Console.WriteLine("==============================================");
                        Console.WriteLine("            DEPARTMENT DETAILS                ");
                        Console.WriteLine("==============================================");
                        Console.WriteLine();

                        Console.Write("Enter Department ID: ");
                        int id = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine();

                        Repo.getDepartmentDetails(id);

                        break;

                    case "3":
                        Console.WriteLine("==============================================");
                        Console.WriteLine("             INSERT DEPARTMENT               ");
                        Console.WriteLine("==============================================");
                        Console.WriteLine();

                        Repo.insertDepartmentByProcedure();

                        break;

                    case "4":
                        Console.WriteLine("==============================================");
                        Console.WriteLine("             UPDATE DEPARTMENT               ");
                        Console.WriteLine("==============================================");
                        Console.WriteLine();

                        Repo.updateDepartmentByProcedure();

                        break;

                    case "5":
                        Console.WriteLine("==============================================");
                        Console.WriteLine("             DELETE DEPARTMENT               ");
                        Console.WriteLine("==============================================");
                        Console.WriteLine();

                        Repo.deleteDepartmentByProcedure();

                        break;

                    case "6":
                        Console.WriteLine("==============================================");
                        Console.WriteLine("     Thank you for using the system!         ");
                        Console.WriteLine("==============================================");

                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please enter 1-6.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
            }
        }
    }
}