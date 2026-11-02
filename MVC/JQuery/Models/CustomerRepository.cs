using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace JQuery.Models
{
    /// <summary>
    /// Data Access Repository utilizing ADO.NET and SQL Server Stored Procedures.
    /// Reads connection string "DBConn" from Web.config and calls dbo.sp_SearchCustomers.
    /// </summary>
    public static class CustomerRepository
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString;

        /// <summary>
        /// Calls the stored procedure dbo.sp_SearchCustomers to retrieve customer records.
        /// When searchTerm is null or empty, the stored procedure returns all customers.
        /// </summary>
        /// <param name="searchTerm">Optional search filter for ContactName, City, or Country</param>
        /// <returns>List of Customer objects populated from SqlDataReader</returns>
        public static List<Customer> GetCustomers(string searchTerm = null)
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.sp_SearchCustomers", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            
                            // Pass @SearchTerm to the stored procedure (or DBNull if null/empty)
                            if (!string.IsNullOrWhiteSpace(searchTerm))
                            {
                                cmd.Parameters.Add("@SearchTerm", SqlDbType.NVarChar, 50).Value = searchTerm.Trim();
                            }
                            else
                            {
                                cmd.Parameters.Add("@SearchTerm", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                            }

                            conn.Open();

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    customers.Add(new Customer
                                    {
                                        CustomerID = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : 0,
                                        ContactName = reader["ContactName"] != DBNull.Value ? reader["ContactName"].ToString() : string.Empty,
                                        City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                        Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
                                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                                        Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback to in-memory seed records if the SQL Server is temporarily unavailable
                System.Diagnostics.Debug.WriteLine("Database query via Stored Procedure failed: " + ex.Message);
                if (customers.Count == 0)
                {
                    customers = GetFallbackCustomers();
                }
            }

            return customers;
        }

        /// <summary>
        /// Calls stored procedure dbo.sp_GetCustomerById to retrieve a single customer by ID.
        /// </summary>
        public static Customer GetCustomerById(int customerId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.sp_GetCustomerById", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;

                            conn.Open();

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return new Customer
                                    {
                                        CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                        ContactName = reader["ContactName"]?.ToString(),
                                        City = reader["City"]?.ToString(),
                                        Country = reader["Country"]?.ToString(),
                                        Email = reader["Email"]?.ToString(),
                                        Phone = reader["Phone"]?.ToString()
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in sp_GetCustomerById: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Calls stored procedure dbo.sp_InsertCustomer to insert a new customer into the database.
        /// </summary>
        public static int InsertCustomer(Customer customer)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.sp_InsertCustomer", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@ContactName", SqlDbType.NVarChar, 100).Value = customer.ContactName ?? string.Empty;
                            cmd.Parameters.Add("@City", SqlDbType.NVarChar, 50).Value = customer.City ?? string.Empty;
                            cmd.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = customer.Country ?? string.Empty;
                            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object)customer.Email ?? DBNull.Value;
                            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object)customer.Phone ?? DBNull.Value;

                            SqlParameter outputId = new SqlParameter("@NewId", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputId);

                            conn.Open();
                            cmd.ExecuteNonQuery();

                            return (int)outputId.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in sp_InsertCustomer: " + ex.Message);
            }

            return 0;
        }

        private static List<Customer> GetFallbackCustomers()
        {
            return new List<Customer>
            {
                new Customer { CustomerID = 1, ContactName = "Maria Anders", City = "Berlin", Country = "Germany", Email = "maria.anders@gmail.com", Phone = "030-0074321" },
                new Customer { CustomerID = 2, ContactName = "Ana Trujillo", City = "Mexico D.F.", Country = "Mexico", Email = "ana.trujillo@yahoo.com", Phone = "(5) 555-4729" },
                new Customer { CustomerID = 3, ContactName = "Antonio Moreno", City = "Mexico D.F.", Country = "Mexico", Email = "antonio.m@gmail.com", Phone = "(5) 555-3932" },
                new Customer { CustomerID = 4, ContactName = "Thomas Hardy", City = "London", Country = "UK", Email = "thomas.hardy@outlook.com", Phone = "(171) 555-7788" },
                new Customer { CustomerID = 5, ContactName = "Christina Berglund", City = "Lulea", Country = "Sweden", Email = "christina.b@domain.se", Phone = "0921-12 34 65" }
            };
        }
    }
}
