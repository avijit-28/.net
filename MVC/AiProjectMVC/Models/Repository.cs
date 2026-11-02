using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AiProjectMVC.Models
{
    /// <summary>
    /// Repository class to perform database operations for the USERS table,
    /// implementing authentication and user lookup as covered in the authorization filter architecture.
    /// </summary>
    public class Repository
    {
        private readonly string _connectionString;

        public Repository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString;
        }

        /// <summary>
        /// Authenticates and retrieves the user details from the USERS table by username/email and password.
        /// </summary>
        public USER GetUserDetails(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            USER user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT TOP 1 UserID, UserName, Roles, Password 
                                 FROM USERS 
                                 WHERE LOWER(UserName) = LOWER(@UserName) AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName.Trim());
                    cmd.Parameters.AddWithValue("@Password", password.Trim());

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new USER
                            {
                                UserID = Convert.ToInt64(reader["UserID"]),
                                UserName = reader["UserName"].ToString(),
                                Roles = reader["Roles"].ToString(),
                                Password = reader["Password"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        /// <summary>
        /// Retrieves a user by their username or email from the USERS table.
        /// </summary>
        public USER GetUserByUsername(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            USER user = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT TOP 1 UserID, UserName, Roles, Password 
                                 FROM USERS 
                                 WHERE LOWER(UserName) = LOWER(@UserName)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName.Trim());
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new USER
                            {
                                UserID = Convert.ToInt64(reader["UserID"]),
                                UserName = reader["UserName"].ToString(),
                                Roles = reader["Roles"].ToString(),
                                Password = reader["Password"].ToString()
                            };
                        }
                    }
                }
            }
            return user;
        }

        /// <summary>
        /// Retrieves all records from the USERS table for display and demo logins.
        /// </summary>
        public List<USER> GetAllUsers()
        {
            var list = new List<USER>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT UserID, UserName, Roles, Password FROM USERS ORDER BY UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new USER
                                {
                                    UserID = Convert.ToInt64(reader["UserID"]),
                                    UserName = reader["UserName"].ToString(),
                                    Roles = reader["Roles"].ToString(),
                                    Password = reader["Password"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                // Return whatever collected or empty list
            }
            return list;
        }
    }
}
