using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class StaffRepository
    {
        public bool Register(RestaurantStaff staff, out string message, out int staffId)
        {
            message = string.Empty;
            staffId = 0;

            // Check duplication
            string checkSql = @"
                SELECT 1 FROM dbo.RestaurantStaff WHERE Email = @Email OR Username = @Username
                UNION
                SELECT 1 FROM dbo.Customers WHERE Email = @Email OR Username = @Username
                UNION
                SELECT 1 FROM dbo.Restaurants WHERE Email = @Email OR Username = @Username
                UNION
                SELECT 1 FROM dbo.Riders WHERE Email = @Email OR Username = @Username";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", staff.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Username", staff.Username.Trim().ToLower());
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

                // Generate Staff Code: STF-R{RestId}-{Seq}
                string countSql = "SELECT COUNT(*) FROM dbo.RestaurantStaff WHERE RestaurantId = @RestId";
                int nextSeq = 1;
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(countSql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestId", staff.RestaurantId);
                    conn.Open();
                    nextSeq = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                }
                staff.StaffCode = $"STF-R{staff.RestaurantId}-{nextSeq:D3}";

                string insertSql = @"
                    INSERT INTO dbo.RestaurantStaff 
                    (StaffCode, StaffName, Username, Email, PasswordHash, PhoneNumber, RestaurantId, RoleTitle, IsVerified, OrdersPrepared, PerformanceRating)
                    VALUES 
                    (@StaffCode, @StaffName, @Username, @Email, @PasswordHash, @PhoneNumber, @RestaurantId, @RoleTitle, 0, 0, 5.0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@StaffCode", staff.StaffCode);
                    cmd.Parameters.AddWithValue("@StaffName", staff.StaffName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", staff.Username.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", staff.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", staff.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", staff.PhoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@RestaurantId", staff.RestaurantId);
                    cmd.Parameters.AddWithValue("@RoleTitle", string.IsNullOrWhiteSpace(staff.RoleTitle) ? "Kitchen Staff" : staff.RoleTitle);

                    conn.Open();
                    staffId = Convert.ToInt32(cmd.ExecuteScalar());
                    message = $"Registration successful! Your generated Staff ID is {staff.StaffCode}. Your account is currently pending verification by restaurant management or admin.";
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
            string sql = @"
                SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId,
                       r.RestaurantName, s.RoleTitle, s.IsVerified, s.OrdersPrepared, s.PerformanceRating, s.IsActive
                FROM dbo.RestaurantStaff s
                INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
                WHERE (s.Email = @LoginId OR s.Username = @LoginId) 
                  AND s.PasswordHash = @Password 
                  AND s.IsActive = 1";

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
                            return new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                OrdersPrepared = Convert.ToInt32(reader["OrdersPrepared"]),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
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
            string sql = @"
                SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId,
                       r.RestaurantName, s.RoleTitle, s.IsVerified, s.OrdersPrepared, s.PerformanceRating, s.IsActive, s.CreatedAt
                FROM dbo.RestaurantStaff s
                INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
                WHERE s.StaffId = @StaffId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
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
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                OrdersPrepared = Convert.ToInt32(reader["OrdersPrepared"]),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
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

        public List<RestaurantStaff> GetByRestaurant(int restaurantId)
        {
            var list = new List<RestaurantStaff>();
            string sql = @"
                SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId,
                       r.RestaurantName, s.RoleTitle, s.IsVerified, s.OrdersPrepared, s.PerformanceRating, s.IsActive, s.CreatedAt
                FROM dbo.RestaurantStaff s
                INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
                WHERE s.RestaurantId = @RestaurantId
                ORDER BY s.StaffId DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
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
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                OrdersPrepared = Convert.ToInt32(reader["OrdersPrepared"]),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
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

        public List<RestaurantStaff> GetAllPendingStaff()
        {
            var list = new List<RestaurantStaff>();
            string sql = @"
                SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId,
                       r.RestaurantName, s.RoleTitle, s.IsVerified, s.OrdersPrepared, s.PerformanceRating, s.IsActive, s.CreatedAt
                FROM dbo.RestaurantStaff s
                INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
                WHERE s.IsVerified = 0 AND s.IsActive = 1
                ORDER BY s.StaffId DESC";

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
                            list.Add(new RestaurantStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                StaffCode = reader["StaffCode"].ToString(),
                                StaffName = reader["StaffName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                RoleTitle = reader["RoleTitle"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                OrdersPrepared = Convert.ToInt32(reader["OrdersPrepared"]),
                                PerformanceRating = Convert.ToDecimal(reader["PerformanceRating"]),
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

        public bool VerifyStaff(int staffId, string verifiedByRole, int verifiedBy, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.RestaurantStaff 
                SET IsVerified = 1, VerifiedByRole = @Role, VerifiedBy = @UserId, VerifiedAt = GETDATE()
                WHERE StaffId = @StaffId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", verifiedByRole ?? "Admin");
                    cmd.Parameters.AddWithValue("@UserId", verifiedBy);
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Staff verified successfully." : "Staff not found.";
                    return ok;
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
            string sql = @"UPDATE dbo.RestaurantStaff SET IsActive = 0 WHERE StaffId = @StaffId";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Staff registration rejected." : "Staff not found.";
                    return ok;
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
            string sql = @"
                INSERT INTO dbo.FoodItems 
                (ItemName, Description, Price, ImageUrl, Category, RestaurantId, SubmittedByStaffId, IsVerified, IsAvailable)
                VALUES 
                (@ItemName, @Description, @Price, @ImageUrl, @Category, @RestaurantId, @StaffId, 0, 1);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)item.ImageUrl ?? "/Content/images/margherita.jpg");
                    cmd.Parameters.AddWithValue("@Category", (object)item.Category ?? "Snacks");
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmd.Parameters.AddWithValue("@StaffId", staffId);

                    conn.Open();
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    message = "New dish proposed successfully! It is pending approval by restaurant owner or administrator.";
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
            var list = new List<FoodItem>();
            string sql = @"
                SELECT FoodItemId, ItemName, Description, Price, ImageUrl, Category, IsAvailable, IsVerified, CreatedAt
                FROM dbo.FoodItems
                WHERE SubmittedByStaffId = @StaffId
                ORDER BY FoodItemId DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
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
                                Category = reader["Category"].ToString(),
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
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

        public bool ApproveDish(int foodItemId, string verifiedByRole, int verifiedBy, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.FoodItems 
                SET IsVerified = 1, VerifiedByRole = @Role, VerifiedBy = @UserId
                WHERE FoodItemId = @FoodItemId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", verifiedByRole ?? "Owner");
                    cmd.Parameters.AddWithValue("@UserId", verifiedBy);
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Dish approved and added to active restaurant menu!" : "Food item not found.";
                    return ok;
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
            string sql = @"UPDATE dbo.FoodItems SET IsAvailable = 0, IsVerified = 0 WHERE FoodItemId = @FoodItemId";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Dish submission rejected." : "Food item not found.";
                    return ok;
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
            string sql = @"
                SELECT f.FoodItemId, f.ItemName, f.Description, f.Price, f.ImageUrl, f.Category, f.IsAvailable, 
                       f.IsVerified, f.CreatedAt, r.RestaurantName, s.StaffName
                FROM dbo.FoodItems f
                LEFT JOIN dbo.Restaurants r ON f.RestaurantId = r.RestaurantId
                LEFT JOIN dbo.RestaurantStaff s ON f.SubmittedByStaffId = s.StaffId
                WHERE f.IsVerified = 0
                  AND (@RestId IS NULL OR f.RestaurantId = @RestId)
                ORDER BY f.FoodItemId DESC";

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
                            list.Add(new FoodItem
                            {
                                FoodItemId = Convert.ToInt32(reader["FoodItemId"]),
                                ItemName = reader["ItemName"].ToString(),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "Snacks",
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                RestaurantName = reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : "-",
                                StaffName = reader["StaffName"] != DBNull.Value ? reader["StaffName"].ToString() : "Staff"
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
                using (var cmd = new SqlCommand("UPDATE dbo.RestaurantStaff SET PasswordHash = @Password WHERE StaffId = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Id", staffId);
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
