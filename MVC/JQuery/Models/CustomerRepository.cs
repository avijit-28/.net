using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace JQuery.Models
{
    /// <summary>
    /// Data access repository communicating with SQL Server using ADO.NET and Stored Procedures.
    /// Reads connection string "DBConn" from Web.config.
    /// </summary>
    public static class CustomerRepository
    {
        // Connection string name configured in Web.config
        private const string ConnectionStringName = "DBConn";

        private static string GetConnectionString()
        {
            var connSetting = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connSetting == null || string.IsNullOrWhiteSpace(connSetting.ConnectionString))
            {
                throw new InvalidOperationException($"Connection string '{ConnectionStringName}' was not found in Web.config.");
            }
            return connSetting.ConnectionString;
        }

        /// <summary>
        /// Fetches customers from SQL Server by executing dbo.sp_SearchCustomers.
        /// When searchTerm is null or empty, returns all customer records.
        /// </summary>
        /// <param name="searchTerm">Optional search keyword for ContactName, City, or Country</param>
        /// <returns>List of Customer entity objects</returns>
        public static List<Customer> GetCustomersFromDatabase(string searchTerm = null)
        {
            var customers = new List<Customer>();
            string connString = GetConnectionString();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    using (var cmd = new SqlCommand("dbo.sp_SearchCustomers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add @SearchTerm parameter
                        if (string.IsNullOrWhiteSpace(searchTerm))
                        {
                            cmd.Parameters.Add("@SearchTerm", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@SearchTerm", SqlDbType.NVarChar, 50).Value = searchTerm.Trim();
                        }

                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var customer = new Customer
                                {
                                    CustomerID = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : 0,
                                    ContactName = reader["ContactName"] != DBNull.Value ? reader["ContactName"].ToString() : string.Empty,
                                    City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                    Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
                                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                                    Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty
                                };

                                customers.Add(customer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In case of SQL connection or execution issue, log and fall back to in-memory data
                System.Diagnostics.Trace.TraceError($"SQL Error fetching customers: {ex.Message}");
                return GetFallbackCustomers();
            }

            return customers;
        }

        /// <summary>
        /// Retrieves a single customer by ID using dbo.sp_GetCustomerById.
        /// </summary>
        public static Customer GetCustomerById(int customerId)
        {
            string connString = GetConnectionString();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    using (var cmd = new SqlCommand("dbo.sp_GetCustomerById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;

                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Customer
                                {
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ContactName = reader["ContactName"].ToString(),
                                    City = reader["City"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                                    Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"SQL Error fetching customer by ID: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Inserts a new customer using dbo.sp_InsertCustomer.
        /// </summary>
        public static int InsertCustomer(Customer customer)
        {
            string connString = GetConnectionString();

            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand("dbo.sp_InsertCustomer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ContactName", SqlDbType.NVarChar, 100).Value = customer.ContactName ?? string.Empty;
                    cmd.Parameters.Add("@City", SqlDbType.NVarChar, 50).Value = customer.City ?? string.Empty;
                    cmd.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = customer.Country ?? string.Empty;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object)customer.Email ?? DBNull.Value;
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object)customer.Phone ?? DBNull.Value;

                    var outParam = new SqlParameter("@NewId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return (int)outParam.Value;
                }
            }
        }

        /// <summary>
        /// Retrieves customers from SQL Server via dbo.sp_SearchCustomers.
        /// </summary>
        public static List<Customer> GetCustomers(string searchTerm = null)
        {
            return GetCustomersFromDatabase(searchTerm);
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
