using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class RestaurantRepository
    {
        public bool Register(Restaurant restaurant, out string message, out int restaurantId)
        {
            message = string.Empty;
            restaurantId = 0;

            // Check duplication
            string checkSql = @"
                SELECT 1 FROM dbo.Restaurants WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.Customers WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.Riders WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.RestaurantStaff WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.Admins WHERE LOWER(Username) = @Username";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", restaurant.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Username", restaurant.Username.Trim().ToLower());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            message = "Username or Email already registered in system.";
                            return false;
                        }
                    }
                }

                string insertSql = @"
                    INSERT INTO dbo.Restaurants 
                    (RestaurantName, OwnerName, Username, Email, PasswordHash, PhoneNumber, Address, Cuisine, Rating, IsVerified, IsActive)
                    VALUES 
                    (@RestaurantName, @OwnerName, @Username, @Email, @PasswordHash, @PhoneNumber, @Address, @Cuisine, 4.5, 0, 1);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantName", restaurant.RestaurantName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@OwnerName", (object)restaurant.OwnerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", restaurant.Username.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", restaurant.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", restaurant.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)restaurant.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)restaurant.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cuisine", (object)restaurant.Cuisine ?? "Multi-Cuisine");

                    conn.Open();
                    restaurantId = Convert.ToInt32(cmd.ExecuteScalar());
                    message = $"Restaurant registration submitted! Your assigned Restaurant ID is #{restaurantId}. Your application is currently under review and will be verified by the admin shortly.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public Restaurant Login(string loginId, string password, out string verificationMessage)
        {
            verificationMessage = string.Empty;
            string sql = @"
                SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, Rating, ImageUrl, IsVerified, IsActive
                FROM dbo.Restaurants
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
                            bool isVerified = Convert.ToBoolean(reader["IsVerified"]);
                            if (!isVerified)
                            {
                                verificationMessage = "Your restaurant application is currently awaiting administrator verification. You will be able to access the kitchen dashboard once approved.";
                                return null;
                            }

                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] != DBNull.Value ? reader["OwnerName"].ToString() : null,
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = isVerified,
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public Restaurant GetById(int restaurantId)
        {
            string sql = @"
                SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, Rating, ImageUrl, IsVerified, IsActive
                FROM dbo.Restaurants
                WHERE RestaurantId = @RestaurantId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] != DBNull.Value ? reader["OwnerName"].ToString() : null,
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<Restaurant> GetAllVerified()
        {
            var list = new List<Restaurant>();
            string sql = @"
                SELECT RestaurantId, RestaurantName, OwnerName, Email, PhoneNumber, Address, Cuisine, Rating, ImageUrl
                FROM dbo.Restaurants
                WHERE IsVerified = 1 AND IsActive = 1
                ORDER BY RestaurantName";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] != DBNull.Value ? reader["OwnerName"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Restaurant> GetPendingVerification()
        {
            var list = new List<Restaurant>();
            string sql = @"
                SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, CreatedAt
                FROM dbo.Restaurants
                WHERE IsVerified = 0 AND IsActive = 1
                ORDER BY RestaurantId DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] != DBNull.Value ? reader["OwnerName"].ToString() : null,
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                IsVerified = false
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool VerifyRestaurant(int restaurantId, int adminId, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.Restaurants
                SET IsVerified = 1, VerifiedBy = @AdminId, VerifiedAt = GETDATE()
                WHERE RestaurantId = @RestaurantId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AdminId", adminId);
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Restaurant verified and approved successfully." : "Restaurant not found.";
                    return ok;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool RejectRestaurant(int restaurantId, out string message)
        {
            message = string.Empty;
            string sql = @"UPDATE dbo.Restaurants SET IsActive = 0 WHERE RestaurantId = @RestaurantId";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Restaurant registration rejected." : "Restaurant not found.";
                    return ok;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool Update(Restaurant model, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.Restaurants
                SET RestaurantName = @RestaurantName,
                    OwnerName = @OwnerName,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address,
                    Cuisine = @Cuisine,
                    ImageUrl = @ImageUrl
                WHERE RestaurantId = @RestaurantId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantId", model.RestaurantId);
                    cmd.Parameters.AddWithValue("@RestaurantName", model.RestaurantName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@OwnerName", (object)model.OwnerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cuisine", (object)model.Cuisine ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)model.ImageUrl ?? DBNull.Value);

                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        message = "Restaurant details updated successfully.";
                        return true;
                    }
                    message = "Restaurant not found.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<Order> GetOrders(int? restaurantId = null)
        {
            var list = new List<Order>();
            string sql = @"
                SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, c.Address, o.TotalAmount, 
                       o.OrderStatus, o.DeliveryStatus, o.OrderedAt, o.ProcessedAt, o.RejectionReason,
                       STUFF((
                           SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                           FROM dbo.OrderDetails od
                           INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                           WHERE od.OrderId = o.OrderId
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
                FROM dbo.Orders o
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                WHERE (@RestId IS NULL OR o.RestaurantId = @RestId)
                ORDER BY o.OrderedAt DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestId", (object)restaurantId ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                DeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : "Pending",
                                OrderedAt = Convert.ToDateTime(reader["OrderedAt"]),
                                ProcessedAt = reader["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ProcessedAt"]) : null,
                                RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : null,
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : null
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool UpdateDeliveryStatus(int orderId, string status)
        {
            string sql = @"UPDATE dbo.Orders SET DeliveryStatus = @Status WHERE OrderId = @OrderId";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public List<Transaction> GetTransactions(int? restaurantId = null)
        {
            var list = new List<Transaction>();
            string sql = @"
                SELECT t.TransactionId, t.OrderId, t.TransactionReference, t.Amount, t.PaymentMethod, 
                       t.PaymentStatus, t.RestaurantShare, t.PlatformFee, t.DeliveryFee, t.CreatedAt,
                       c.CustomerName, r.RestaurantName
                FROM dbo.Transactions t
                INNER JOIN dbo.Orders o ON t.OrderId = o.OrderId
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                INNER JOIN dbo.Restaurants r ON o.RestaurantId = r.RestaurantId
                WHERE (@RestId IS NULL OR o.RestaurantId = @RestId)
                ORDER BY t.TransactionId DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestId", (object)restaurantId ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Transaction
                            {
                                TransactionId = Convert.ToInt32(reader["TransactionId"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                TransactionReference = reader["TransactionReference"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                PaymentStatus = reader["PaymentStatus"].ToString(),
                                RestaurantShare = Convert.ToDecimal(reader["RestaurantShare"]),
                                PlatformFee = Convert.ToDecimal(reader["PlatformFee"]),
                                DeliveryFee = Convert.ToDecimal(reader["DeliveryFee"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool ResetPassword(int restaurantId, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.Restaurants SET PasswordHash = @Password WHERE RestaurantId = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Id", restaurantId);
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
