using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalDepartment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DoctorsRepo repo = new DoctorsRepo();
            Console.WriteLine("Welcome to our Medical Center");
            while (true)
            {
                Console.WriteLine("\n======== Doctors Detaills =======================================================================");
                Console.WriteLine("1. Add Doctor");
                Console.WriteLine("2. Show Doctors");
                Console.WriteLine("3. Updates Doctors Details");
                //Console.WriteLine("4. Delete Doctors Details");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    //Doctors doctor = new Doctors();
                    Console.Write("Enter First Name: ");
                    string firstName = Console.ReadLine();
                    Console.Write("Enter Last Name: ");
                    string lastname = Console.ReadLine();
                    Console.Write("Enter Specialization: ");
                    string specialization = Console.ReadLine();
                    Console.Write("Enter Department ID: ");
                    int departmentId = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Enter Phone Number: ");
                    string phoneNumber = Console.ReadLine();
                    Doctors doctor = new Doctors { FirstName = firstName, Lastname = lastname, Specialization = specialization, DepartmentId = departmentId, Email = email, PhoneNumber = phoneNumber };
                    repo.addDoctor(doctor);
                    Console.WriteLine("Doctor added successfully!");
                }
                else if (choice == "2")
                {
                    repo.showDoctors();
                }

                else if (choice == "3")
                {
                    repo.updateDoctorsDetails();
                }
                else if (choice == "4")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }

            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();

        }
    }
}
