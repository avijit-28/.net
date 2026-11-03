using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class CustomerRepository
    {
        public bool CheckDuplication(string username, string email, out string message)
        {
            message = string.Empty;
            string checkSql = @"
                SELECT 'Username already taken' AS Reason FROM dbo.Customers WHERE LOWER(Username) = @Username
                UNION ALL
                SELECT 'Email already registered' FROM dbo.Customers WHERE LOWER(Email) = @Email
                UNION ALL
                SELECT 'Username already taken' FROM dbo.Restaurants WHERE LOWER(Username) = @Username
                UNION ALL
                SELECT 'Email already registered' FROM dbo.Restaurants WHERE LOWER(Email) = @Email
                UNION ALL
                SELECT 'Username already taken' FROM dbo.Riders WHERE LOWER(Username) = @Username
                UNION ALL
                SELECT 'Email already registered' FROM dbo.Riders WHERE LOWER(Email) = @Email
                UNION ALL
                SELECT 'Username already taken' FROM dbo.RestaurantStaff WHERE LOWER(Username) = @Username
                UNION ALL
                SELECT 'Email already registered' FROM dbo.RestaurantStaff WHERE LOWER(Email) = @Email
                UNION ALL
                SELECT 'Username already taken' FROM dbo.Admins WHERE LOWER(Username) = @Username";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", (username ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLower());
                    conn.Open();
                    var reason = cmd.ExecuteScalar()?.ToString();
                    if (!string.IsNullOrEmpty(reason))
                    {
                        message = reason;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return true;
            }

            return false;
        }

        public bool Insert(Customer customer, out string message, out int customerId)
        {
            message = string.Empty;
            customerId = 0;

            if (CheckDuplication(customer.Username, customer.Email, out message))
            {
                return false;
            }

            string sql = @"
                INSERT INTO dbo.Customers (CustomerName, Username, Email, PasswordHash, PhoneNumber, Address)
                VALUES (@CustomerName, @Username, @Email, @PasswordHash, @PhoneNumber, @Address);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", (customer.Username ?? customer.Email).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", customer.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", customer.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);

                    conn.Open();
                    customerId = Convert.ToInt32(cmd.ExecuteScalar());
                    message = "Customer registered successfully.";
                    return true;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    message = "Email or Username already exists.";
                }
                else
                {
                    message = ex.Message;
                }
                return false;
            }
        }

        public Customer Login(string loginId, string password)
        {
            string sql = @"
                SELECT CustomerId, CustomerName, Username, Email, PhoneNumber, Address, IsActive
                FROM dbo.Customers
                WHERE (LOWER(Email) = @LoginId OR LOWER(Username) = @LoginId)
                  AND PasswordHash = @Password
                  AND IsActive = 1";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LoginId", (loginId ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Password", password ?? string.Empty);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                IsActive = true
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<Customer> GetAll()
        {
            var list = new List<Customer>();
            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_GetAll");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"] != DBNull.Value ? row["PhoneNumber"].ToString() : null,
                    Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                });
            }
            return list;
        }

        public Customer GetById(int customerId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_GetById", parameters);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"] != DBNull.Value ? row["PhoneNumber"].ToString() : null,
                    Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                };
            }
            return null;
        }

        public bool Update(Customer customer, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customer.CustomerId),
                new SqlParameter("@CustomerName", customer.CustomerName ?? string.Empty),
                new SqlParameter("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value),
                new SqlParameter("@Address", (object)customer.Address ?? DBNull.Value)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_Update", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public bool Delete(int customerId, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_Delete", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public bool ResetPassword(int customerId, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.Customers SET PasswordHash = @Password WHERE CustomerId = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        message = "Password reset successfully.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}
