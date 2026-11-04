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

        public bool Insert(RestaurantStaff staff, out string message, out int staffId, out string staffCode)
        {
            message = string.Empty;
            staffId = 0;
            staffCode = string.Empty;

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
                    cmd.Parameters.AddWithValue("@StaffName", staff.StaffName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", (staff.Username ?? staff.Email).Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", staff.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", staff.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", staff.PhoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@RestaurantId", staff.RestaurantId);
                    cmd.Parameters.AddWithValue("@RoleTitle", (object)staff.RoleTitle ?? "Kitchen Staff");

                    var idParam = new SqlParameter("@StaffId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var codeParam = new SqlParameter("@StaffCode", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.Parameters.Add(codeParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (idParam.Value != DBNull.Value && idParam.Value != null)
                    {
                        staffId = Convert.ToInt32(idParam.Value);
                    }
                    if (codeParam.Value != DBNull.Value && codeParam.Value != null)
                    {
                        staffCode = codeParam.Value.ToString();
                    }

                    message = $"Kitchen staff registered successfully with Staff ID: {staffCode}. Pending restaurant/admin verification.";
                    return true;
                }
            }
            catch (SqlException ex)
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
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"] != DBNull.Value ? reader["RoleTitle"].ToString() : "Staff",
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

        public RestaurantStaff GetById(int id)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"] != DBNull.Value ? reader["RoleTitle"].ToString() : "Staff",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<RestaurantStaff> GetRestaurantStaff(int restaurantId)
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
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"] != DBNull.Value ? reader["RoleTitle"].ToString() : "Staff",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
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
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"] != DBNull.Value ? reader["RoleTitle"].ToString() : "Staff",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
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
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"] != DBNull.Value ? reader["RoleTitle"].ToString() : "Staff",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
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

        public bool VerifyStaff(int staffId, bool isApproved)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_Verify", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
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
                    cmd.Parameters.AddWithValue("@Category", (object)item.Category ?? "Snacks");
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)item.ImageUrl ?? "/Content/images/margherita.jpg");
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmd.Parameters.AddWithValue("@ProposedByStaffId", staffId);

                    var idParam = new SqlParameter("@FoodItemId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    message = "Dish proposed successfully! It will appear on menu after restaurant owner approval.";
                    return true;
                }
            }
            catch (SqlException ex)
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
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "Snacks",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                RestaurantId = reader["RestaurantId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["RestaurantId"]) : null,
                                RestaurantName = reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : null,
                                ProposedByStaffName = reader["ProposedByStaffName"] != DBNull.Value ? reader["ProposedByStaffName"].ToString() : null,
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool VerifyDish(int foodItemId, bool isApproved)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_VerifyDish", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool ToggleAvailability(int foodItemId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Staff_ToggleDishAvailability", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public List<FoodItem> GetStaffDishes(int staffId)
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
