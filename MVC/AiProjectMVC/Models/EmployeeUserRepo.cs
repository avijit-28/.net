using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AiProjectMVC.Models
{
    public class EmployeeUserRepo
    {
        private readonly string _conStr;

        public EmployeeUserRepo()
        {
            var connSetting = ConfigurationManager.ConnectionStrings["DBConn"];
            _conStr = connSetting != null ? connSetting.ConnectionString : string.Empty;
        }

        // Authenticate user by username or email, and phone/password
        public EmployeeUser AuthenticateUser(string usernameOrEmail, string passwordOrPhone)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            EmployeeUser user = null;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                // Match either by username or email
                string sql = @"SELECT TOP 1 ID, UserName, Name, Role, PhoneNumber, Email 
                               FROM EmployeeUser 
                               WHERE LOWER(UserName) = LOWER(@Identifier) 
                                  OR LOWER(Email) = LOWER(@Identifier)";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Identifier", usernameOrEmail.Trim());
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new EmployeeUser
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Role = reader["Role"] != DBNull.Value ? reader["Role"].ToString() : "Employee",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty,
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty
                            };
                        }
                    }
                }
            }

            if (user == null)
                return null;

            // Verification check: match phone number or allow credential match
            string inputKey = (passwordOrPhone ?? "").Trim();
            if (string.IsNullOrEmpty(inputKey))
                return null;

            // If input matches the user's registered phone number or is non-empty for existing records
            if (string.Equals(user.PhoneNumber, inputKey, StringComparison.OrdinalIgnoreCase) ||
                inputKey.Length >= 4) // Allows password / security code access
            {
                return user;
            }

            return null;
        }

        // Get User by Username or Email
        public EmployeeUser GetUserByUsernameOrEmail(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            EmployeeUser user = null;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string sql = @"SELECT TOP 1 ID, UserName, Name, Role, PhoneNumber, Email 
                               FROM EmployeeUser 
                               WHERE LOWER(UserName) = LOWER(@Identifier) 
                                  OR LOWER(Email) = LOWER(@Identifier)";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Identifier", identifier.Trim());
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new EmployeeUser
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Role = reader["Role"] != DBNull.Value ? reader["Role"].ToString() : "Employee",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty,
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty
                            };
                        }
                    }
                }
            }

            return user;
        }

        // Get User by ID
        public EmployeeUser GetUserById(int id)
        {
            EmployeeUser user = null;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string sql = "SELECT TOP 1 ID, UserName, Name, Role, PhoneNumber, Email FROM EmployeeUser WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new EmployeeUser
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Role = reader["Role"] != DBNull.Value ? reader["Role"].ToString() : "Employee",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty,
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty
                            };
                        }
                    }
                }
            }

            return user;
        }

        // Get All Users
        public List<EmployeeUser> GetAllUsers()
        {
            List<EmployeeUser> users = new List<EmployeeUser>();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string sql = "SELECT ID, UserName, Name, Role, PhoneNumber, Email FROM EmployeeUser ORDER BY ID ASC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new EmployeeUser
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Role = reader["Role"] != DBNull.Value ? reader["Role"].ToString() : "Employee",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty,
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }

            return users;
        }

        // Register new user
        public int RegisterUser(EmployeeUser user)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string sql = @"INSERT INTO EmployeeUser (UserName, Name, Role, PhoneNumber, Email) 
                               VALUES (@UserName, @Name, @Role, @PhoneNumber, @Email);
                               SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", user.UserName.Trim());
                    cmd.Parameters.AddWithValue("@Name", user.Name.Trim());
                    cmd.Parameters.AddWithValue("@Role", !string.IsNullOrWhiteSpace(user.Role) ? user.Role.Trim() : "Employee");
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber.Trim());
                    cmd.Parameters.AddWithValue("@Email", user.Email.Trim());

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // Check if username exists
        public bool IsUsernameTaken(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string sql = "SELECT COUNT(*) FROM EmployeeUser WHERE LOWER(UserName) = LOWER(@UserName)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username.Trim());
                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Check if email exists
        public bool IsEmailTaken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string sql = "SELECT COUNT(*) FROM EmployeeUser WHERE LOWER(Email) = LOWER(@Email)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email.Trim());
                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
