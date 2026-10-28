using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AiProjectMVC.Models
{
    public class EmployeeRepo
    {
        private readonly string _conStr;

        public EmployeeRepo()
        {
            var connSetting = ConfigurationManager.ConnectionStrings["DBConn"];
            _conStr = connSetting != null ? connSetting.ConnectionString : string.Empty;
        }

        public List<Employee> GetAllEmployee()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Salary = reader["Salary"] != DBNull.Value ? Convert.ToInt32(reader["Salary"]) : 0,
                                Gender = reader["Gender"] != DBNull.Value ? reader["Gender"].ToString() : string.Empty,
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                Dept = reader["Dept"] != DBNull.Value ? reader["Dept"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }

            return employees;
        }

        public Employee GetEmployeeById(int id)
        {
            Employee employee = null;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetEmployeeById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            employee = new Employee
                            {
                                ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Salary = reader["Salary"] != DBNull.Value ? Convert.ToInt32(reader["Salary"]) : 0,
                                Gender = reader["Gender"] != DBNull.Value ? reader["Gender"].ToString() : string.Empty,
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                Dept = reader["Dept"] != DBNull.Value ? reader["Dept"].ToString() : string.Empty
                            };
                        }
                    }
                }
            }

            return employee;
        }

        public void InsertEmployee(Employee employee)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("InsertEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", employee.ID);
                    cmd.Parameters.AddWithValue("@Name", (object)employee.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    cmd.Parameters.AddWithValue("@Gender", (object)employee.Gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)employee.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dept", (object)employee.Dept ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEmployee(
            int id,
            string name = null,
            decimal? salary = null,
            string gender = null,
            string city = null,
            string dept = null)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name", (object)name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary", (object)salary ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)city ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dept", (object)dept ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteEmployee(int id)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
