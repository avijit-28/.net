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

        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            try { return reader.GetOrdinal(columnName) >= 0; }
            catch { return false; }
        }

        private static Restaurant MapRestaurant(SqlDataReader reader)
        {
            var r = new Restaurant();
            r.RestaurantId = Convert.ToInt32(reader["RestaurantId"]);
            r.RestaurantName = reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : string.Empty;
            r.OwnerName = HasColumn(reader, "OwnerName") && reader["OwnerName"] != DBNull.Value ? reader["OwnerName"].ToString() : string.Empty;
            r.Username = HasColumn(reader, "Username") && reader["Username"] != DBNull.Value ? reader["Username"].ToString() : string.Empty;
            r.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty;
            r.PhoneNumber = HasColumn(reader, "PhoneNumber") && reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty;
            r.Address = HasColumn(reader, "Address") && reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty;
            r.Cuisine = HasColumn(reader, "Cuisine") && reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : string.Empty;
            r.Rating = HasColumn(reader, "Rating") && reader["Rating"] != DBNull.Value ? Convert.ToDecimal(reader["Rating"]) : 4.5m;
            r.ImageUrl = HasColumn(reader, "ImageUrl") && reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : string.Empty;
            r.IsVerified = HasColumn(reader, "IsVerified") && reader["IsVerified"] != DBNull.Value && Convert.ToBoolean(reader["IsVerified"]);
            r.IsActive = !HasColumn(reader, "IsActive") || reader["IsActive"] == DBNull.Value || Convert.ToBoolean(reader["IsActive"]);
            r.Latitude = HasColumn(reader, "Latitude") && reader["Latitude"] != DBNull.Value ? Convert.ToDecimal(reader["Latitude"]) : (decimal?)null;
            r.Longitude = HasColumn(reader, "Longitude") && reader["Longitude"] != DBNull.Value ? Convert.ToDecimal(reader["Longitude"]) : (decimal?)null;
            return r;
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
                            var rest = MapRestaurant(reader);
                            if (!rest.IsVerified)
                            {
                                verificationMessage = "Your restaurant account is pending admin verification. You will receive an update once verified.";
                                return null;
                            }
                            return rest;
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
                            return MapRestaurant(reader);
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
                            list.Add(MapRestaurant(reader));
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
                            list.Add(MapRestaurant(reader));
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
                            list.Add(MapRestaurant(reader));
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
                            var ord = new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                OrderNumber = HasColumn(reader, "OrderNumber") && reader["OrderNumber"] != DBNull.Value ? reader["OrderNumber"].ToString() : ("FR" + (100000 + Convert.ToInt32(reader["OrderId"]))),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = HasColumn(reader, "CustomerName") && reader["CustomerName"] != DBNull.Value ? reader["CustomerName"].ToString() : "Customer",
                                CustomerPhone = HasColumn(reader, "CustomerPhone") && reader["CustomerPhone"] != DBNull.Value ? reader["CustomerPhone"].ToString() : (HasColumn(reader, "PhoneNumber") && reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                DeliveryAddress = HasColumn(reader, "DeliveryAddress") && reader["DeliveryAddress"] != DBNull.Value ? reader["DeliveryAddress"].ToString() : (HasColumn(reader, "Address") && reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty),
                                PaymentMethod = HasColumn(reader, "PaymentMethod") && reader["PaymentMethod"] != DBNull.Value ? reader["PaymentMethod"].ToString() : "UPI",
                                PaymentStatus = HasColumn(reader, "PaymentStatus") && reader["PaymentStatus"] != DBNull.Value ? reader["PaymentStatus"].ToString() : "Pending",
                                OrderStatus = HasColumn(reader, "OrderStatus") && reader["OrderStatus"] != DBNull.Value ? reader["OrderStatus"].ToString() : "Pending",
                                DeliveryStatus = HasColumn(reader, "DeliveryStatus") && reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : "Pending",
                                CreatedAt = HasColumn(reader, "CreatedAt") && reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (HasColumn(reader, "OrderedAt") && reader["OrderedAt"] != DBNull.Value ? Convert.ToDateTime(reader["OrderedAt"]) : DateTime.Now),
                                RestaurantId = HasColumn(reader, "RestaurantId") && reader["RestaurantId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["RestaurantId"]) : null,
                                RestaurantName = HasColumn(reader, "RestaurantName") && reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : string.Empty,
                                SubOrderPreparationStatus = HasColumn(reader, "SubOrderPreparationStatus") && reader["SubOrderPreparationStatus"] != DBNull.Value ? reader["SubOrderPreparationStatus"].ToString() : string.Empty,
                                SubOrderAmount = HasColumn(reader, "SubOrderAmount") && reader["SubOrderAmount"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["SubOrderAmount"]) : null,
                                Items = HasColumn(reader, "Items") && reader["Items"] != DBNull.Value ? reader["Items"].ToString() : string.Empty
                            };
                            list.Add(ord);
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