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
            string sql = @"
                SELECT TOP 15 NotificationId, UserId, UserRole, Title, Message, NotificationType, IsRead, CreatedAt, TargetUrl
                FROM dbo.Notifications
                WHERE (UserRole = 'All' OR UserRole = @Role)
                  AND (@UserId IS NULL OR UserId IS NULL OR UserId = @UserId)
                ORDER BY CreatedAt DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", (object)role ?? "All");
                    cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
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
            string sql = @"
                UPDATE dbo.Notifications
                SET IsRead = 1
                WHERE (UserRole = 'All' OR UserRole = @Role)
                  AND (@UserId IS NULL OR UserId IS NULL OR UserId = @UserId)";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", (object)role ?? "All");
                    cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
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
