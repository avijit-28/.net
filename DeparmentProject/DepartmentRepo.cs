using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeparmentProject
{
    public class DepartmentRepo
    {
        
        public string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        //List<Department> departmentList = new List<Department>();

        public void getDepartmentDetails(int id)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            using (SqlCommand command = new SqlCommand("get_DepartmentDetails", conn))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@id", id);

                SqlParameter messageParameter =
                    command.Parameters.Add("@msg", SqlDbType.VarChar, 200);

                messageParameter.Direction = ParameterDirection.Output;

                conn.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"Department ID: {reader["department_id"]} | " +
                            $"Department Name: {reader["department_name"]} | " +
                            $"Head of Department: {reader["head_of_department"]} | " +
                            $"Phone Extension: {reader["phone_extension"]}"
                        );
                    }
                }
                Console.WriteLine(messageParameter.Value);
            }
        }
        public void getallDepartmentByProcedure()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("get_AllDepartmentDetails", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter messageParameter = new SqlParameter("@msg", SqlDbType.VarChar, 200);

                    messageParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(messageParameter);

                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"Department ID: {reader["department_id"]} | " +
                                $"Department Name: {reader["department_name"]} | " +
                                $"Head of Department: {reader["head_of_department"]} | " +
                                $"Phone Extension: {reader["phone_extension"]}"
                            );
                        }
                    }
                    reader.Close();
                    Console.WriteLine(messageParameter.Value.ToString());
                }
            }
        }

        public void insertDepartmentByProcedure()
        {
            Console.Write("Department Name: ");
            string departmentName = Console.ReadLine();

            Console.Write("Head of Department: ");
            string hod = Console.ReadLine();

            Console.Write("Phone Extension: ");
            string phoneExtension = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(conStr))
            using (SqlCommand command = new SqlCommand("set_InsertDepartmentDetails", conn))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@department_name", departmentName);
                command.Parameters.AddWithValue("@hod", hod);
                command.Parameters.AddWithValue("@phone_ext", phoneExtension);

                SqlParameter messageParameter =
                    command.Parameters.Add("@msg", SqlDbType.VarChar, 200);

                messageParameter.Direction = ParameterDirection.Output;

                conn.Open();

                command.ExecuteNonQuery();

                Console.WriteLine(messageParameter.Value);
            }
        }

        public void deleteDepartmentByProcedure()
        {
            Console.Write("Department Id: ");
            int id = Convert.ToInt32(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(conStr))
            using (SqlCommand command = new SqlCommand("set_DeleteDepartmentDetails", conn))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@id", id);

                SqlParameter messageParameter =
                    command.Parameters.Add("@msg", SqlDbType.VarChar, 200);

                messageParameter.Direction = ParameterDirection.Output;

                conn.Open();

                command.ExecuteNonQuery();

                Console.WriteLine(messageParameter.Value);
            }
        }

        public void updateDepartmentByProcedure()
        {
            Console.Write("Department Id: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Department Name: ");
            string departmentName = Console.ReadLine();

            Console.Write("Head of Department: ");
            string hod = Console.ReadLine();

            Console.Write("Phone Extension: ");
            string phoneExtension = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(conStr))
            using (SqlCommand command = new SqlCommand("UpdateDepartmentDetails", conn))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@department_name",string.IsNullOrWhiteSpace(departmentName)? (object)DBNull.Value: departmentName);
                command.Parameters.AddWithValue("@hod", string.IsNullOrWhiteSpace(hod)? (object)DBNull.Value: hod);
                command.Parameters.AddWithValue("@Phone_ext", string.IsNullOrWhiteSpace(phoneExtension)? (object)DBNull.Value: phoneExtension);

                SqlParameter messageParameter =
                    command.Parameters.Add("@msg", SqlDbType.VarChar, 200);

                messageParameter.Direction = ParameterDirection.Output;

                conn.Open();

                command.ExecuteNonQuery();
                getDepartmentDetails(id);



                Console.WriteLine(messageParameter.Value);
            }
        }
    }
}
