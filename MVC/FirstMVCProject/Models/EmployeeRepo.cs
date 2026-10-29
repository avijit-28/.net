using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FirstMVCProject.Models
{
    public class EmployeeRepo
    {
        private readonly string _conStr =
        ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;

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
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                Salary = Convert.ToInt32(reader["Salary"]),
                                Gender = reader["Gender"].ToString(),
                                City = reader["City"].ToString(),
                                Dept = reader["Dept"].ToString(),
                                AddressId = reader["AddressId"] == DBNull.Value
                            ? (int?)null
                            : Convert.ToInt32(reader["AddressId"])
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
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                Salary = Convert.ToInt32(reader["Salary"]),
                                Gender = reader["Gender"].ToString(),
                                City = reader["City"].ToString(),
                                Dept = reader["Dept"].ToString(),
                                AddressId = reader["AddressId"] == DBNull.Value
                            ? (int?)null
                            : Convert.ToInt32(reader["AddressId"])
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
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                    cmd.Parameters.AddWithValue("@City", employee.City);
                    cmd.Parameters.AddWithValue("@Dept", employee.Dept);
                    cmd.Parameters.AddWithValue("@AddressId",
                    (object)employee.AddressId ?? DBNull.Value);

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
       string dept = null,
        int? addressId = null)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name",
                        (object)name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary",
                        (object)salary ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender",
                        (object)gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City",
                        (object)city ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dept",
                        (object)dept ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressId",
                        (object)addressId ?? DBNull.Value);

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