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
                    cmd.Parameters.AddWithValue("@OwnerName", restaurant.OwnerName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", (restaurant.Username ?? restaurant.Email).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", restaurant.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", restaurant.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", restaurant.PhoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Address", restaurant.Address ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Cuisine", (object)restaurant.Cuisine ?? "Multicuisine");
                    cmd.Parameters.AddWithValue("@Latitude", restaurant.Latitude != 0 ? restaurant.Latitude : 22.572600m);
                    cmd.Parameters.AddWithValue("@Longitude", restaurant.Longitude != 0 ? restaurant.Longitude : 88.363900m);

                    var idParam = new SqlParameter("@RestaurantId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (idParam.Value != DBNull.Value && idParam.Value != null)
                    {
                        restaurantId = Convert.ToInt32(idParam.Value);
                    }
                    message = "Restaurant registered successfully. Pending administrator verification.";
                    return true;
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public Restaurant Login(string loginId, string password)
        {
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
                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : 22.572600m,
                                Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : 88.363900m
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
                                OwnerName = reader["OwnerName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : 22.572600m,
                                Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : 88.363900m,
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
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
                                OwnerName = reader["OwnerName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = true,
                                IsActive = true,
                                Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : 22.572600m,
                                Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : 88.363900m,
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
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
                                OwnerName = reader["OwnerName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = false,
                                IsActive = true,
                                Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : 22.572600m,
                                Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : 88.363900m,
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public Restaurant GetById(int id)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : 22.572600m,
                                Longitude = reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : 88.363900m,
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public bool VerifyRestaurant(int restaurantId, bool isApproved)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_Verify", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
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
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "Snacks",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Order> GetOrders(int restaurantId)
        {
            var list = new List<Order>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetOrders", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
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
                                RiderName = reader["RiderName"] != DBNull.Value ? reader["RiderName"].ToString() : null,
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : "Items"
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

        public List<Transaction> GetRestaurantTransactions(int restaurantId)
        {
            var list = new List<Transaction>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Restaurant_GetTransactions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
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
