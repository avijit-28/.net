using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class RestaurantRepository
    {
        public bool CheckDuplication(string username, string email, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_User_CheckDuplication", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", (username ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLower());

                    var outParam = new SqlParameter("@Reason", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (outParam.Value != DBNull.Value && outParam.Value != null)
                    {
                        string reason = outParam.Value.ToString();
                        if (!string.IsNullOrEmpty(reason))
                        {
                            message = reason;
                            return true;
                        }
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

        public bool Register(Restaurant restaurant, out string message, out int restaurantId)
        {
            return Insert(restaurant, out message, out restaurantId);
        }

        public bool Insert(Restaurant restaurant, out string message, out int restaurantId)
        {
            message = string.Empty;
            restaurantId = 0;

            if (CheckDuplication(restaurant.Username, restaurant.Email, out message))
            {
                return false;
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantName", restaurant.RestaurantName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@OwnerName", (object)restaurant.OwnerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", (restaurant.Username ?? restaurant.Email).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", restaurant.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", restaurant.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)restaurant.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)restaurant.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cuisine", (object)restaurant.Cuisine ?? "Multi-Cuisine");
                    cmd.Parameters.AddWithValue("@Latitude", (object)restaurant.Latitude ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Longitude", (object)restaurant.Longitude ?? DBNull.Value);

                    var outId = new SqlParameter("@RestaurantId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (outId.Value != DBNull.Value && outId.Value != null)
                    {
                        restaurantId = Convert.ToInt32(outId.Value);
                    }

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

        public Restaurant Login(string loginId, string password)
        {
            string dummy;
            return Login(loginId, password, out dummy);
        }

        public Restaurant Login(string loginId, string password, out string verificationMessage)
        {
            verificationMessage = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
                                verificationMessage = "Your restaurant account is pending admin verification. You will receive an update once verified.";
                                return null;
                            }

                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                Username = reader["Username"] == DBNull.Value ? string.Empty : reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] == DBNull.Value ? string.Empty : reader["Cuisine"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] == DBNull.Value ? string.Empty : reader["ImageUrl"].ToString(),
                                IsVerified = isVerified,
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Latitude"]),
                                Longitude = reader["Longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Longitude"])
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                Username = reader["Username"] == DBNull.Value ? string.Empty : reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] == DBNull.Value ? string.Empty : reader["Cuisine"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] == DBNull.Value ? string.Empty : reader["ImageUrl"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Latitude"]),
                                Longitude = reader["Longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Longitude"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<Restaurant> GetAll()
        {
            var list = new List<Restaurant>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                Username = reader["Username"] == DBNull.Value ? string.Empty : reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] == DBNull.Value ? string.Empty : reader["Cuisine"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] == DBNull.Value ? string.Empty : reader["ImageUrl"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Latitude"]),
                                Longitude = reader["Longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Longitude"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Restaurant> GetAllVerified()
        {
            var list = new List<Restaurant>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetAllVerified", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                Username = reader["Username"] == DBNull.Value ? string.Empty : reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] == DBNull.Value ? string.Empty : reader["Cuisine"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] == DBNull.Value ? string.Empty : reader["ImageUrl"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Latitude"]),
                                Longitude = reader["Longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Longitude"])
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetPendingVerification", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                Username = reader["Username"] == DBNull.Value ? string.Empty : reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] == DBNull.Value ? string.Empty : reader["Cuisine"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] == DBNull.Value ? string.Empty : reader["ImageUrl"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Latitude"]),
                                Longitude = reader["Longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Longitude"])
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_Verify", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmd.Parameters.AddWithValue("@AdminId", adminId);
                    cmd.Parameters.AddWithValue("@IsApproved", true);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Restaurant verified successfully!";
                    return true;
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_Reject", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Restaurant application rejected.";
                    return true;
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", model.RestaurantId);
                    cmd.Parameters.AddWithValue("@RestaurantName", model.RestaurantName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@OwnerName", (object)model.OwnerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cuisine", (object)model.Cuisine ?? "Multi-Cuisine");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Restaurant profile updated successfully.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<FoodItem> GetMenu(int restaurantId)
        {
            var list = new List<FoodItem>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetMenu", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new FoodItem
                            {
                                FoodItemId = Convert.ToInt32(reader["FoodItemId"]),
                                ItemName = reader["ItemName"].ToString(),
                                Description = reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImageUrl = reader["ImageUrl"] == DBNull.Value ? string.Empty : reader["ImageUrl"].ToString(),
                                Category = reader["Category"].ToString(),
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Order> GetOrders(int? restaurantId = null)
        {
            var list = new List<Order>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetOrders", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", (object)restaurantId ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                OrderNumber = reader["OrderNumber"].ToString(),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                CustomerPhone = reader["CustomerPhone"].ToString(),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                DeliveryAddress = reader["DeliveryAddress"].ToString(),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                PaymentStatus = reader["PaymentStatus"].ToString(),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                DeliveryStatus = reader["DeliveryStatus"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                RestaurantId = reader["RestaurantId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"] == DBNull.Value ? string.Empty : reader["RestaurantName"].ToString(),
                                SubOrderPreparationStatus = reader["SubOrderPreparationStatus"] == DBNull.Value ? string.Empty : reader["SubOrderPreparationStatus"].ToString(),
                                SubOrderAmount = reader["SubOrderAmount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["SubOrderAmount"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool UpdateDeliveryStatus(int orderId, string status, int? restaurantId = null)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_UpdateDeliveryStatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@RestaurantId", (object)restaurantId ?? DBNull.Value);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public List<Transaction> GetTransactions(int? restaurantId = null)
        {
            var list = new List<Transaction>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetTransactions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", (object)restaurantId ?? DBNull.Value);
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
                                CustomerName = reader["CustomerName"] == DBNull.Value ? string.Empty : reader["CustomerName"].ToString(),
                                RestaurantName = reader["RestaurantName"] == DBNull.Value ? string.Empty : reader["RestaurantName"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Transaction> GetRestaurantTransactions(int restaurantId)
        {
            return GetTransactions(restaurantId);
        }

        public bool ResetPassword(int restaurantId, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_ResetPassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    conn.Open();
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());
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