using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FirstADOConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=.;Initial Catalog=Hospital;User ID=sa;Password=mcc#1234;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                string query = " SELECT  [department_id],[department_name],[head_of_department],[phone_extension] FROM [Hospital].[hpl].[Departments] ";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["department_id"]}, Department Name: {reader["department_name"]}, Head od Department: {reader["head_of_department"]}, Phone extension: {reader["phone_extension"]}");
                    }
                }
        
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();

        }
    }
}
