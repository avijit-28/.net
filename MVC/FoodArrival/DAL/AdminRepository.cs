using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class AdminRepository
    {
        public Admin Login(string loginId, string password)
        {
            string cleanId = (loginId ?? string.Empty).Trim().ToLower();
            string cleanPass = (password ?? string.Empty).Trim();

            string sql = @"
                SELECT AdminId, AdminName, Username, Email, IsActive
                FROM dbo.Admins
                WHERE (LOWER(Email) = @LoginId OR LOWER(Username) = @LoginId)
                  AND PasswordHash = @Password
                  AND IsActive = 1";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LoginId", cleanId);
                    cmd.Parameters.AddWithValue("@Password", cleanPass);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Admin
                            {
                                AdminId = Convert.ToInt32(reader["AdminId"]),
                                AdminName = reader["AdminName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                IsActive = true
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public AdminDashboardViewModel GetDashboardSummary()
        {
            var vm = new AdminDashboardViewModel();

            string summarySql = @"
                SELECT 
                    (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Pending') AS PendingOrders,
                    (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Accepted') AS AcceptedOrders,
                    (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Rejected') AS RejectedOrders,
                    (SELECT COUNT(*) FROM dbo.FoodItems WHERE IsAvailable = 1 AND IsVerified = 1) AS AvailableFoodItems,
                    (SELECT COUNT(*) FROM dbo.Customers WHERE IsActive = 1) AS ActiveCustomers,
                    (SELECT COUNT(*) FROM dbo.Restaurants WHERE IsActive = 1 AND IsVerified = 1) AS TotalRestaurants,
                    (SELECT COUNT(*) FROM dbo.Riders WHERE IsActive = 1) AS TotalRiders,
                    (SELECT COUNT(*) FROM dbo.RestaurantStaff WHERE IsActive = 1 AND IsVerified = 1) AS TotalStaff,
                    (
                        (SELECT COUNT(*) FROM dbo.Restaurants WHERE IsVerified = 0 AND IsActive = 1) +
                        (SELECT COUNT(*) FROM dbo.RestaurantStaff WHERE IsVerified = 0 AND IsActive = 1) +
                        (SELECT COUNT(*) FROM dbo.RiderRestaurantAffiliations WHERE IsVerified = 0) +
                        (SELECT COUNT(*) FROM dbo.FoodItems WHERE IsVerified = 0)
                    ) AS PendingApprovalsCount,
                    ISNULL((SELECT SUM(Amount) FROM dbo.Transactions WHERE PaymentStatus = 'Success'), 0) AS TotalRevenue,
                    ISNULL((SELECT SUM(PlatformFee) FROM dbo.Transactions WHERE PaymentStatus = 'Success'), 0) AS PlatformEarnings";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(summarySql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vm.PendingOrders = Convert.ToInt32(reader["PendingOrders"]);
                            vm.AcceptedOrders = Convert.ToInt32(reader["AcceptedOrders"]);
                            vm.RejectedOrders = Convert.ToInt32(reader["RejectedOrders"]);
                            vm.AvailableFoodItems = Convert.ToInt32(reader["AvailableFoodItems"]);
                            vm.ActiveCustomers = Convert.ToInt32(reader["ActiveCustomers"]);
                            vm.TotalRestaurants = Convert.ToInt32(reader["TotalRestaurants"]);
                            vm.TotalRiders = Convert.ToInt32(reader["TotalRiders"]);
                            vm.TotalStaff = Convert.ToInt32(reader["TotalStaff"]);
                            vm.PendingApprovalsCount = Convert.ToInt32(reader["PendingApprovalsCount"]);
                            vm.TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"]);
                            vm.PlatformEarnings = Convert.ToDecimal(reader["PlatformEarnings"]);
                        }
                    }
                }
            }
            catch { }

            return vm;
        }

        public List<Transaction> GetAllTransactions()
        {
            var list = new List<Transaction>();
            string sql = @"
                SELECT t.TransactionId, t.OrderId, t.TransactionReference, t.Amount, t.PaymentMethod, 
                       t.PaymentStatus, t.RestaurantShare, t.PlatformFee, t.DeliveryFee, t.CreatedAt,
                       c.CustomerName, r.RestaurantName
                FROM dbo.Transactions t
                INNER JOIN dbo.Orders o ON t.OrderId = o.OrderId
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                LEFT JOIN dbo.Restaurants r ON o.RestaurantId = r.RestaurantId
                ORDER BY t.TransactionId DESC";

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
                                RestaurantName = reader["RestaurantName"] != DBNull.Value ? reader["RestaurantName"].ToString() : "Central Kitchen",
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Bulletin> GetBulletins(string targetAudience = null)
        {
            var list = new List<Bulletin>();
            string sql = @"
                SELECT BulletinId, Title, Message, TargetAudience, Priority, CreatedByAdminId, CreatedAt, IsActive
                FROM dbo.Bulletins
                WHERE IsActive = 1
                  AND (@Audience IS NULL OR TargetAudience = 'All' OR TargetAudience = @Audience)
                ORDER BY BulletinId DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Audience", (object)targetAudience ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Bulletin
                            {
                                BulletinId = Convert.ToInt32(reader["BulletinId"]),
                                Title = reader["Title"].ToString(),
                                Message = reader["Message"].ToString(),
                                TargetAudience = reader["TargetAudience"].ToString(),
                                Priority = reader["Priority"].ToString(),
                                CreatedByAdminId = Convert.ToInt32(reader["CreatedByAdminId"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool AddBulletin(Bulletin bulletin)
        {
            string sql = @"
                INSERT INTO dbo.Bulletins (Title, Message, TargetAudience, Priority, CreatedByAdminId)
                VALUES (@Title, @Message, @TargetAudience, @Priority, @AdminId)";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", bulletin.Title ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Message", bulletin.Message ?? string.Empty);
                    cmd.Parameters.AddWithValue("@TargetAudience", bulletin.TargetAudience ?? "All");
                    cmd.Parameters.AddWithValue("@Priority", bulletin.Priority ?? "Normal");
                    cmd.Parameters.AddWithValue("@AdminId", bulletin.CreatedByAdminId > 0 ? bulletin.CreatedByAdminId : 1);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool DeleteBulletin(int id)
        {
            string sql = @"UPDATE dbo.Bulletins SET IsActive = 0 WHERE BulletinId = @Id";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }
    }
}
