using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class NotificationRepository
    {
        public List<Notification> GetNotifications(string role = "All", int? userId = null)
        {
            var list = new List<Notification>();
            if (string.IsNullOrWhiteSpace(role) || role.Equals("All", StringComparison.OrdinalIgnoreCase) || role.Equals("Guest", StringComparison.OrdinalIgnoreCase))
            {
                // Unauthenticated visitors do not receive private notifications
                return list;
            }

            string sql;
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                // Admin views platform administrative notifications
                sql = @"
                    SELECT TOP 15 NotificationId, UserId, UserRole, Title, Message, NotificationType, IsRead, CreatedAt, TargetUrl
                    FROM dbo.Notifications
                    WHERE UserRole = 'Admin'
                    ORDER BY CreatedAt DESC";
            }
            else
            {
                // For Customer, Rider, Restaurant Owner, and Staff: STRICTLY show ONLY their own personal notifications!
                if (!userId.HasValue || userId.Value <= 0)
                {
                    return list;
                }

                sql = @"
                    SELECT TOP 15 NotificationId, UserId, UserRole, Title, Message, NotificationType, IsRead, CreatedAt, TargetUrl
                    FROM dbo.Notifications
                    WHERE UserRole = @Role AND UserId = @UserId
                    ORDER BY CreatedAt DESC";
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", (object)role ?? "All");
                    if (userId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId.Value);
                    }
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Notification
                            {
                                NotificationId = Convert.ToInt32(reader["NotificationId"]),
                                UserId = reader["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["UserId"]) : null,
                                UserRole = reader["UserRole"].ToString(),
                                Title = reader["Title"].ToString(),
                                Message = reader["Message"].ToString(),
                                NotificationType = reader["NotificationType"].ToString(),
                                IsRead = Convert.ToBoolean(reader["IsRead"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                TargetUrl = reader["TargetUrl"] != DBNull.Value ? reader["TargetUrl"].ToString() : null
                            });
                        }
                    }
                }
            }
            catch
            {
                // return empty list on db error
            }

            return list;
        }

        public bool AddNotification(string role, int? userId, string title, string message, string type = "Info", string targetUrl = null)
        {
            string sql = @"
                INSERT INTO dbo.Notifications (UserId, UserRole, Title, Message, NotificationType, TargetUrl)
                VALUES (@UserId, @UserRole, @Title, @Message, @Type, @TargetUrl)";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserRole", role ?? "All");
                    cmd.Parameters.AddWithValue("@Title", title ?? "FoodReady Alert");
                    cmd.Parameters.AddWithValue("@Message", message ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Type", type ?? "Info");
                    cmd.Parameters.AddWithValue("@TargetUrl", (object)targetUrl ?? DBNull.Value);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool MarkAllAsRead(string role, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;

            string sql;
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                sql = "UPDATE dbo.Notifications SET IsRead = 1 WHERE UserRole = 'Admin'";
            }
            else
            {
                if (!userId.HasValue || userId.Value <= 0) return false;
                sql = "UPDATE dbo.Notifications SET IsRead = 1 WHERE UserRole = @Role AND UserId = @UserId";
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", role);
                    if (userId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId.Value);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
