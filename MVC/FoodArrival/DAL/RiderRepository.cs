using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class RiderRepository
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

        public bool Insert(Rider rider, out string message, out int riderId)
        {
            message = string.Empty;
            riderId = 0;

            if (CheckDuplication(rider.Username, rider.Email, out message))
            {
                return false;
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderName", rider.RiderName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", (rider.Username ?? rider.Email).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", rider.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", rider.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", rider.PhoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@VehicleType", rider.VehicleType ?? "Motorbike");
                    cmd.Parameters.AddWithValue("@LicenseNumber", (object)rider.LicenseNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LicenseDocumentUrl", (object)rider.LicenseDocumentUrl ?? DBNull.Value);

                    var idParam = new SqlParameter("@RiderId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (idParam.Value != DBNull.Value && idParam.Value != null)
                    {
                        riderId = Convert.ToInt32(idParam.Value);
                    }
                    message = "Rider profile registered successfully. You can now affiliate with partner restaurants.";
                    return true;
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool AddAffiliation(int riderId, int restaurantId, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_AddAffiliation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    var affIdParam = new SqlParameter("@AffiliationId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var existsParam = new SqlParameter("@AlreadyExists", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(affIdParam);
                    cmd.Parameters.Add(existsParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (existsParam.Value != DBNull.Value && Convert.ToBoolean(existsParam.Value))
                    {
                        message = "You have already registered/requested affiliation with this restaurant.";
                        return false;
                    }

                    message = "Affiliation request submitted. Pending verification by the restaurant or administrator.";
                    return true;
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public Rider Login(string loginId, string password)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginId", (loginId ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Password", password ?? string.Empty);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rider
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                CurrentStatus = reader["CurrentStatus"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                Latitude = reader["Latitude"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Latitude"]) : null,
                                Longitude = reader["Longitude"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Longitude"]) : null,
                                MaxCapacity = reader["MaxCapacity"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxCapacity"]) : null,
                                Rating = reader["Rating"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Rating"]) : null
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<Rider> GetAll()
        {
            var list = new List<Rider>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Rider
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                CurrentStatus = reader["CurrentStatus"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public Rider GetById(int id)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rider
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                CurrentStatus = reader["CurrentStatus"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                Latitude = reader["Latitude"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Latitude"]) : null,
                                Longitude = reader["Longitude"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Longitude"]) : null,
                                MaxCapacity = reader["MaxCapacity"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxCapacity"]) : null,
                                Rating = reader["Rating"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Rating"]) : null
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<RiderAffiliation> GetRiderAffiliations(int riderId)
        {
            var list = new List<RiderAffiliation>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetAffiliations", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RiderAffiliation
                            {
                                AffiliationId = Convert.ToInt32(reader["AffiliationId"]),
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"].ToString(),
                                RestaurantAddress = reader["RestaurantAddress"] != DBNull.Value ? reader["RestaurantAddress"].ToString() : null,
                                RestaurantPhone = reader["RestaurantPhone"] != DBNull.Value ? reader["RestaurantPhone"].ToString() : null,
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                VerifiedByRole = reader["VerifiedByRole"] != DBNull.Value ? reader["VerifiedByRole"].ToString() : null,
                                VerifiedAt = reader["VerifiedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["VerifiedAt"]) : null,
                                CreatedAt = Convert.ToDateTime(reader["AffiliatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<RiderAffiliation> GetRestaurantRiders(int restaurantId)
        {
            var list = new List<RiderAffiliation>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetByRestaurant", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RiderAffiliation
                            {
                                AffiliationId = Convert.ToInt32(reader["AffiliationId"]),
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RiderName = reader["RiderName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                CurrentStatus = reader["CurrentStatus"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                VerifiedByRole = reader["VerifiedByRole"] != DBNull.Value ? reader["VerifiedByRole"].ToString() : null,
                                VerifiedAt = reader["VerifiedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["VerifiedAt"]) : null,
                                CreatedAt = Convert.ToDateTime(reader["AffiliatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<RiderAffiliation> GetPendingAffiliations()
        {
            var list = new List<RiderAffiliation>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetPendingAffiliations", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RiderAffiliation
                            {
                                AffiliationId = Convert.ToInt32(reader["AffiliationId"]),
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RiderName = reader["RiderName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                RestaurantName = reader["RestaurantName"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                CreatedAt = Convert.ToDateTime(reader["AffiliatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool VerifyAffiliation(int affiliationId, bool isApproved)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_VerifyAffiliation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AffiliationId", affiliationId);
                    cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool RemoveAffiliation(int riderId, int restaurantId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_RemoveAffiliation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public List<Order> GetAvailableDeliveries()
        {
            var list = new List<Order>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetAvailableDeliveries", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
                                DeliveryAddress = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                DeliveryStatus = reader["DeliveryStatus"].ToString(),
                                OrderedAt = Convert.ToDateTime(reader["OrderedAt"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : "Items"
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Order> GetMyDeliveries(int riderId)
        {
            var list = new List<Order>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetMyDeliveries", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
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
                                DeliveryAddress = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                DeliveryStatus = reader["DeliveryStatus"].ToString(),
                                OrderedAt = Convert.ToDateTime(reader["OrderedAt"]),
                                ProcessedAt = reader["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ProcessedAt"]) : null,
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : "Items"
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool AcceptDelivery(int orderId, int riderId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_AcceptDelivery", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool CompleteDelivery(int orderId, int riderId, string enteredOtp, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(enteredOtp))
            {
                errorMessage = "Please enter the 6-digit delivery OTP provided by the customer.";
                return false;
            }

            enteredOtp = enteredOtp.Trim();
            if (enteredOtp.Length != 6)
            {
                errorMessage = "Delivery OTP must be exactly 6 digits.";
                return false;
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_CompleteDeliveryWithOtp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@EnteredOtp", enteredOtp);

                    var errParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 300)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var successParam = new SqlParameter("@IsSuccess", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(errParam);
                    cmd.Parameters.Add(successParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    bool isSuccess = successParam.Value != DBNull.Value && Convert.ToBoolean(successParam.Value);
                    if (!isSuccess)
                    {
                        errorMessage = errParam.Value != DBNull.Value ? errParam.Value.ToString() : "Delivery verification failed.";
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Database error verifying OTP: " + ex.Message;
                return false;
            }
        }

        public bool CompleteDelivery(int orderId, int riderId)
        {
            string msg;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Order_GetDeliveryOtp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    var obj = cmd.ExecuteScalar();
                    string otp = obj != null && obj != DBNull.Value ? obj.ToString() : "";
                    return CompleteDelivery(orderId, riderId, otp, out msg);
                }
            }
            catch { return false; }
        }

        public bool ResetPassword(int riderId, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_ResetPassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
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
