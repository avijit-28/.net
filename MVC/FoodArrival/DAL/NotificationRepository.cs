using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class NotificationRepository
    {
        public List<Notification> GetNotifications(string role, int? userId = null)
        {
            var list = new List<Notification>();
            if (string.IsNullOrWhiteSpace(role))
            {
                return list;
            }

            // Non-admin roles strictly require a valid UserId
            if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (!userId.HasValue || userId.Value <= 0)
                {
                    return list;
                }
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Notification_GetForUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Role", role);
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
            catch { }

            return list;
        }

        public bool AddNotification(string role, int? userId, string title, string message, string type = "Info", string targetUrl = null)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Notification_Add", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Notification_MarkAllAsRead", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Role", role);
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
