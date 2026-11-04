using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class StaffRepository
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

        public bool Register(RestaurantStaff staff, out string message, out int staffId)
        {
            return Insert(staff, out message, out staffId);
        }

        public bool Insert(RestaurantStaff staff, out string message, out int staffId)
        {
            message = string.Empty;
            staffId = 0;

            if (CheckDuplication(staff.Username, staff.Email, out message))
            {
                return false;
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullName", staff.FullName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", (staff.Username ?? staff.Email).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", staff.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", staff.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)staff.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RestaurantId", staff.RestaurantId);
                    cmd.Parameters.AddWithValue("@RoleTitle", (object)staff.RoleTitle ?? "Kitchen Staff");

                    var outStaffId = new SqlParameter("@StaffId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outStaffId);

                    var outCode = new SqlParameter("@StaffCode", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCode);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (outStaffId.Value != DBNull.Value && outStaffId.Value != null)
                    {
                        staffId = Convert.ToInt32(outStaffId.Value);
                    }
                    string staffCode = outCode.Value?.ToString() ?? $"STF-{staffId:D4}";

                    message = $"Registration successful! Your official Staff ID is '{staffCode}'. Verification is required before you can access all kitchen duties.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public RestaurantStaff Login(string loginId, string password)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginId", (loginId ?? string.Empty).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Password", password ?? string.Empty);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
                                OrdersPreparedCount = Convert.ToInt32(reader["OrdersPreparedCount"]),
                                ShiftStatus = reader["ShiftStatus"].ToString(),
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

        public RestaurantStaff GetById(int staffId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
                                OrdersPreparedCount = Convert.ToInt32(reader["OrdersPreparedCount"]),
                                ShiftStatus = reader["ShiftStatus"].ToString(),
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

        public List<RestaurantStaff> GetByRestaurant(int restaurantId)
        {
            var list = new List<RestaurantStaff>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetByRestaurant", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
                                OrdersPreparedCount = Convert.ToInt32(reader["OrdersPreparedCount"]),
                                ShiftStatus = reader["ShiftStatus"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<RestaurantStaff> GetAll()
        {
            var list = new List<RestaurantStaff>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
                                OrdersPreparedCount = Convert.ToInt32(reader["OrdersPreparedCount"]),
                                ShiftStatus = reader["ShiftStatus"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<RestaurantStaff> GetAllPendingStaff()
        {
            return GetPendingVerification();
        }

        public List<RestaurantStaff> GetPendingVerification()
        {
            var list = new List<RestaurantStaff>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetPendingVerification", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
                                OrdersPreparedCount = Convert.ToInt32(reader["OrdersPreparedCount"]),
                                ShiftStatus = reader["ShiftStatus"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool VerifyStaff(int staffId, string verifiedByRole, int verifiedBy, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_Verify", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    cmd.Parameters.AddWithValue("@VerifiedByRole", verifiedByRole ?? "Admin");
                    cmd.Parameters.AddWithValue("@VerifiedBy", verifiedBy);
                    cmd.Parameters.AddWithValue("@IsApproved", true);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Staff member successfully verified!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool RejectStaff(int staffId, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_Reject", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Staff application rejected.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool ProposeDish(FoodItem item, int staffId, int restaurantId, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_ProposeDish", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)item.ImageUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", item.Category ?? "Snacks");
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmd.Parameters.AddWithValue("@SubmittedByStaffId", staffId);

                    var outParam = new SqlParameter("@NewFoodItemId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int newId = outParam.Value != DBNull.Value ? Convert.ToInt32(outParam.Value) : 0;
                    message = $"New dish '{item.ItemName}' proposed successfully! (ID: #{newId}) It will appear on the menu once approved by restaurant owner or admin.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<FoodItem> GetStaffDishes(int staffId)
        {
            return GetDishesByStaff(staffId);
        }

        public List<FoodItem> GetDishesByStaff(int staffId)
        {
            var list = new List<FoodItem>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetDishesByStaff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
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

        public bool ApproveDish(int foodItemId, string verifiedByRole, int verifiedBy, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_VerifyDish", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    cmd.Parameters.AddWithValue("@VerifiedByRole", verifiedByRole ?? "Admin");
                    cmd.Parameters.AddWithValue("@VerifiedBy", verifiedBy);
                    cmd.Parameters.AddWithValue("@IsApproved", true);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Dish approved and activated for ordering!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool RejectDish(int foodItemId, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_RejectDish", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    message = "Proposed dish has been rejected.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<FoodItem> GetPendingDishes(int? restaurantId = null)
        {
            var list = new List<FoodItem>();
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetPendingDishes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RestaurantId", (object)restaurantId ?? DBNull.Value);
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

        public bool ToggleDishAvailability(int foodItemId, bool isAvailable)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_ToggleDishAvailability", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool ResetPassword(int staffId, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_ResetPassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
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