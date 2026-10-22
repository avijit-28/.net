using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication4
{
    /// <summary>
    /// Data Access Layer (DAL) for managing UserDetails using SQL Server Stored Procedures.
    /// Interacts directly with stored procedures:
    /// - sp_GetAllUserDetails
    /// - sp_InsertUserDetails
    /// - sp_UpdateUserDetails
    /// - sp_DeleteUserDetails
    /// </summary>
    public class UserDetailsAccessLayer
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;
        }

        /// <summary>
        /// Retrieves all user records using stored procedure sp_GetAllUserDetails.
        /// </summary>
        public static List<UserDetails> GetAllUsers()
        {
            List<UserDetails> listUsers = new List<UserDetails>();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllUserDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            UserDetails user = new UserDetails
                            {
                                UserId = Convert.ToInt32(rdr["UserId"]),
                                UserName = rdr["UserName"] != DBNull.Value ? rdr["UserName"].ToString() : string.Empty,
                                Password = rdr["Password"] != DBNull.Value ? rdr["Password"].ToString() : string.Empty,
                                Email = rdr["Email"] != DBNull.Value ? rdr["Email"].ToString() : string.Empty,
                                MobileNo = rdr["MobileNo"] != DBNull.Value ? rdr["MobileNo"].ToString() : string.Empty,
                                City = rdr["City"] != DBNull.Value ? rdr["City"].ToString() : string.Empty
                            };
                            listUsers.Add(user);
                        }
                    }
                }
            }
            return listUsers;
        }

        /// <summary>
        /// Inserts a new user record using stored procedure sp_InsertUserDetails.
        /// If UserId is 0 or unassigned, generates the next available primary key automatically.
        /// </summary>
        public static void InsertUser(int UserId, string UserName, string Password, string Email, string MobileNo, string City)
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();

                if (UserId <= 0)
                {
                    using (SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(UserId), 0) + 1 FROM UserDetails", con))
                    {
                        UserId = Convert.ToInt32(cmdMax.ExecuteScalar());
                    }
                }

                using (SqlCommand cmd = new SqlCommand("sp_InsertUserDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@UserName", (object)UserName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", (object)Password ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)City ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates an existing user record using stored procedure sp_UpdateUserDetails.
        /// </summary>
        public static void UpdateUser(int UserId, string UserName, string Password, string Email, string MobileNo, string City)
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateUserDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@UserName", (object)UserName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", (object)Password ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)City ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes a user record by primary key using stored procedure sp_DeleteUserDetails.
        /// </summary>
        public static void DeleteUser(int UserId)
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteUserDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
