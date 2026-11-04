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

        public bool Register(Rider rider, int restaurantId, out string message, out int riderId)
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
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)rider.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", (object)rider.VehicleType ?? "Motorbike");
                    cmd.Parameters.AddWithValue("@LicenseNumber", (object)rider.LicenseNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LicenseDocumentUrl", (object)rider.LicenseDocumentUrl ?? DBNull.Value);

                    var outRiderId = new SqlParameter("@RiderId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outRiderId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (outRiderId.Value != DBNull.Value && outRiderId.Value != null)
                    {
                        riderId = Convert.ToInt32(outRiderId.Value);
                    }
                }

                if (riderId > 0 && restaurantId > 0)
                {
                    string affMsg;
                    AddAffiliation(riderId, restaurantId, out affMsg);
                }

                message = $"Rider account registered successfully! Your account and restaurant partner affiliation are pending verification by the restaurant or admin.";
                return true;
            }
            catch (Exception ex)
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

                    var outAffId = new SqlParameter("@AffiliationId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outAffId);

                    var outExists = new SqlParameter("@AlreadyExists", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outExists);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    bool alreadyExists = outExists.Value != DBNull.Value && Convert.ToBoolean(outExists.Value);
                    if (alreadyExists)
                    {
                        message = "You have already applied or are affiliated with this restaurant.";
                        return false;
                    }

                    message = "Affiliation application submitted. Pending review and approval.";
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

        private static Rider MapRider(SqlDataReader reader)
        {
            var r = new Rider();
            r.RiderId = Convert.ToInt32(reader["RiderId"]);
            r.RiderName = reader["RiderName"] != DBNull.Value ? reader["RiderName"].ToString() : string.Empty;
            r.Username = HasColumn(reader, "Username") && reader["Username"] != DBNull.Value ? reader["Username"].ToString() : string.Empty;
            r.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty;
            r.PhoneNumber = HasColumn(reader, "PhoneNumber") && reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty;
            r.VehicleType = HasColumn(reader, "VehicleType") && reader["VehicleType"] != DBNull.Value ? reader["VehicleType"].ToString() : "Motorbike";
            r.LicenseNumber = HasColumn(reader, "LicenseNumber") && reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : string.Empty;
            r.LicenseDocumentUrl = HasColumn(reader, "LicenseDocumentUrl") && reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : string.Empty;
            r.CurrentStatus = HasColumn(reader, "CurrentStatus") && reader["CurrentStatus"] != DBNull.Value ? reader["CurrentStatus"].ToString() : "Available";
            r.IsActive = !HasColumn(reader, "IsActive") || reader["IsActive"] == DBNull.Value || Convert.ToBoolean(reader["IsActive"]);
            return r;
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
                            return MapRider(reader);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public Rider GetById(int riderId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapRider(reader);
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
                            list.Add(MapRider(reader));
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool Update(Rider model, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RiderId", model.RiderId);
                    cmd.Parameters.AddWithValue("@RiderName", model.RiderName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", (object)model.VehicleType ?? "Motorbike");
                    cmd.Parameters.AddWithValue("@LicenseNumber", (object)model.LicenseNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LicenseDocumentUrl", (object)model.LicenseDocumentUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrentStatus", (object)model.CurrentStatus ?? "Available");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Rider profile updated successfully.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
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
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                RestaurantAddress = reader["RestaurantAddress"] == DBNull.Value ? string.Empty : reader["RestaurantAddress"].ToString(),
                                RestaurantPhone = reader["RestaurantPhone"] == DBNull.Value ? string.Empty : reader["RestaurantPhone"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                VerifiedByRole = reader["VerifiedByRole"] == DBNull.Value ? string.Empty : reader["VerifiedByRole"].ToString(),
                                VerifiedBy = reader["VerifiedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["VerifiedBy"]),
                                VerifiedAt = reader["VerifiedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VerifiedAt"]),
                                AffiliatedAt = Convert.ToDateTime(reader["AffiliatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<RiderAffiliation> GetRestaurantRiders(int? restaurantId = null, bool? onlyPending = null)
        {
            var list = new List<RiderAffiliation>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_GetRestaurantRiders", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", (object)restaurantId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OnlyPending", (object)onlyPending ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RiderAffiliation
                            {
                                AffiliationId = Convert.ToInt32(reader["AffiliationId"]),
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"] == DBNull.Value ? "Motorbike" : reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] == DBNull.Value ? string.Empty : reader["LicenseNumber"].ToString(),
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] == DBNull.Value ? string.Empty : reader["LicenseDocumentUrl"].ToString(),
                                CurrentStatus = reader["CurrentStatus"] == DBNull.Value ? "Available" : reader["CurrentStatus"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                OwnerName = reader["OwnerName"] == DBNull.Value ? string.Empty : reader["OwnerName"].ToString(),
                                RestaurantAddress = reader["RestaurantAddress"] == DBNull.Value ? string.Empty : reader["RestaurantAddress"].ToString(),
                                RestaurantPhone = reader["RestaurantPhone"] == DBNull.Value ? string.Empty : reader["RestaurantPhone"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                VerifiedByRole = reader["VerifiedByRole"] == DBNull.Value ? string.Empty : reader["VerifiedByRole"].ToString(),
                                VerifiedBy = reader["VerifiedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["VerifiedBy"]),
                                VerifiedAt = reader["VerifiedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VerifiedAt"]),
                                AffiliatedAt = Convert.ToDateTime(reader["AffiliatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool VerifyAffiliation(int affiliationId, string role, int userId, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_VerifyAffiliation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AffiliationId", affiliationId);
                    cmd.Parameters.AddWithValue("@VerifiedByRole", role ?? "Admin");
                    cmd.Parameters.AddWithValue("@VerifiedBy", userId);
                    cmd.Parameters.AddWithValue("@IsApproved", true);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Rider affiliation successfully verified!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool RejectAffiliation(int affiliationId, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_RejectAffiliation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AffiliationId", affiliationId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Rider affiliation rejected and removed.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
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
                                OrderNumber = HasColumn(reader, "OrderNumber") && reader["OrderNumber"] != DBNull.Value ? reader["OrderNumber"].ToString() : ("FR" + (100000 + Convert.ToInt32(reader["OrderId"]))),
                                CustomerName = HasColumn(reader, "CustomerName") && reader["CustomerName"] != DBNull.Value ? reader["CustomerName"].ToString() : "Customer",
                                CustomerPhone = HasColumn(reader, "CustomerPhone") && reader["CustomerPhone"] != DBNull.Value ? reader["CustomerPhone"].ToString() : (HasColumn(reader, "PhoneNumber") && reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty),
                                DeliveryAddress = HasColumn(reader, "DeliveryAddress") && reader["DeliveryAddress"] != DBNull.Value ? reader["DeliveryAddress"].ToString() : (HasColumn(reader, "Address") && reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = HasColumn(reader, "OrderStatus") && reader["OrderStatus"] != DBNull.Value ? reader["OrderStatus"].ToString() : "Accepted",
                                DeliveryStatus = HasColumn(reader, "DeliveryStatus") && reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : "ReadyForPickup",
                                CreatedAt = HasColumn(reader, "CreatedAt") && reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (HasColumn(reader, "OrderedAt") && reader["OrderedAt"] != DBNull.Value ? Convert.ToDateTime(reader["OrderedAt"]) : DateTime.Now),
                                RestaurantName = HasColumn(reader, "RestaurantName") && reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : "Partner Kitchen",
                                Items = HasColumn(reader, "Items") && reader["Items"] != DBNull.Value ? reader["Items"].ToString() : string.Empty
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
                                OrderNumber = HasColumn(reader, "OrderNumber") && reader["OrderNumber"] != DBNull.Value ? reader["OrderNumber"].ToString() : ("FR" + (100000 + Convert.ToInt32(reader["OrderId"]))),
                                CustomerName = HasColumn(reader, "CustomerName") && reader["CustomerName"] != DBNull.Value ? reader["CustomerName"].ToString() : "Customer",
                                CustomerPhone = HasColumn(reader, "CustomerPhone") && reader["CustomerPhone"] != DBNull.Value ? reader["CustomerPhone"].ToString() : (HasColumn(reader, "PhoneNumber") && reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : string.Empty),
                                DeliveryAddress = HasColumn(reader, "DeliveryAddress") && reader["DeliveryAddress"] != DBNull.Value ? reader["DeliveryAddress"].ToString() : (HasColumn(reader, "Address") && reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = HasColumn(reader, "OrderStatus") && reader["OrderStatus"] != DBNull.Value ? reader["OrderStatus"].ToString() : "Accepted",
                                DeliveryStatus = HasColumn(reader, "DeliveryStatus") && reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : "OutForDelivery",
                                CreatedAt = HasColumn(reader, "CreatedAt") && reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (HasColumn(reader, "OrderedAt") && reader["OrderedAt"] != DBNull.Value ? Convert.ToDateTime(reader["OrderedAt"]) : DateTime.Now),
                                RestaurantName = HasColumn(reader, "RestaurantName") && reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : "Partner Kitchen",
                                DeliveryOtp = HasColumn(reader, "DeliveryOtp") && reader["DeliveryOtp"] != DBNull.Value ? reader["DeliveryOtp"].ToString() : null,
                                Items = HasColumn(reader, "Items") && reader["Items"] != DBNull.Value ? reader["Items"].ToString() : string.Empty
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
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool CompleteDelivery(int orderId, int riderId, string enteredOtp, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Rider_CompleteDeliveryWithOtp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@EnteredOtp", (object)enteredOtp ?? DBNull.Value);

                    var outMsg = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 300)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outMsg);

                    var outSuccess = new SqlParameter("@IsSuccess", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outSuccess);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    bool isSuccess = outSuccess.Value != DBNull.Value && Convert.ToBoolean(outSuccess.Value);
                    if (!isSuccess)
                    {
                        errorMessage = outMsg.Value?.ToString() ?? "Invalid Delivery OTP. Delivery not confirmed.";
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool CompleteDelivery(int orderId, int riderId)
        {
            string err;
            return CompleteDelivery(orderId, riderId, null, out err);
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