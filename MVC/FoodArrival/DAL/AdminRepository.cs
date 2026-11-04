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

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Admin_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Admin_GetDashboardSummary", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Admin_GetAllTransactions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Admin_GetBulletins", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Admin_AddBulletin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Admin_DeleteBulletin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BulletinId", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }
    }
}
