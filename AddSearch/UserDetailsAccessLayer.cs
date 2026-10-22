using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace AddSearch
{
	

    public class UserDetailsAccessLayer
	{
        private readonly static string _conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        public static List<UserDetails> GetAllUsers()
		{

			List<UserDetails> listUsers = new List<UserDetails>();
			using (SqlConnection con = new SqlConnection(_conStr))
			{
                SqlCommand cmd = new SqlCommand("sp_GetAllUserDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserDetails user = new UserDetails();
                    user.UserId = Convert.ToInt32(rdr["UserId"]);
                    user.UserName = rdr["UserName"]?.ToString();
                    user.Password = rdr["Password"]?.ToString();
                    user.Email = rdr["Email"]?.ToString();
                    user.MobileNo = rdr["MobileNo"]?.ToString();
                    user.City = rdr["City"]?.ToString();
                    listUsers.Add(user);
                }
            }
            return listUsers;
        }


        public static void InsertUser(
            int UserId,
            string UserName,
            string Password,
            string Email,
            string MobileNo,
            string City
            )
        {
 
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertUserDetails", con);
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

        public static void UpdateUser(int UserId, string UserName, string Password, string Email, string MobileNo, string City)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateUserDetails", con);
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

        public static void DeleteUser(int UserId)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteUserDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}

