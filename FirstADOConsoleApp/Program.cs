using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace FirstADOConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string connectionString = "Data Source=.;Initial Catalog=Hospital;User ID=sa;Password=mcc#1234;";
            //string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
            //using (SqlConnection connection = new SqlConnection(conStr))
            //using (var connection = new SqlConnection(conStr))
            //{
            //connection.Open();
            //Console.WriteLine("Connection to database sucessfully");
            //string query = " SELECT top(10)  [department_id],[department_name],[head_of_department],[phone_extension] FROM [Hospital].[hpl].[Departments] ";
            //string query = "INSERT INTO hpl.Departments VALUES\r\n( 'xyz', 'Dr. Anuj Sharma', '1028')";
            //SqlCommand command = new SqlCommand(query, connection);
            //connection.Open();
            //using (SqlDataReader reader = command.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        Console.WriteLine($"ID: {reader["department_id"]}, Department Name: {reader["department_name"]}, Head of Department: {reader["head_of_department"]}, Phone extension: {reader["phone_extension"]}");
            //    }
            //}
            //while (reader.Read())
            //{
            //    Console.WriteLine(reader["department_id"] + " " + reader["department_name"]);
            //}
            //}
            //int rowsAffected = command.ExecuteNonQuery();
            //Console.WriteLine("Inserted Rows = " + rowsAffected);


            //string  query2= "update hpl.departments set head_of_department ='Dr. Anubhaw Sharma' where department_id = 3";
            //command.CommandText = query2;
            //int rowsAffected2 = command.ExecuteNonQuery();
            //Console.WriteLine("Updated Rows =" + rowsAffected2);


            //string query3 = "delete from hpl.departments where department_id = 21";
            //command.CommandText = query3;
            //int rowsAffected3 = command.ExecuteNonQuery();
            //Console.WriteLine("Deleted Rows =" + rowsAffected3);
            //}
            DoctorsRepo repo = new DoctorsRepo();
            Console.WriteLine("Welcome to our Medical Center");
            while (true)
            {
                Console.WriteLine("1. Add Doctor");
                Console.WriteLine("2. Show Doctors");
                Console.WriteLine("3. Exit");
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
                    string phoneNumber =Console.ReadLine();
                    Doctors doctor = new Doctors { FirstName = firstName, Lastname = lastname, Specialization = specialization, DepartmentId = departmentId, Email = email, PhoneNumber = phoneNumber };
                    repo.addDoctor(doctor);
                    Console.WriteLine("Doctor added successfully!");
                }
                else if (choice == "2")
                {
                    repo.showDoctors();
                }
                else if(choice == "3")
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
