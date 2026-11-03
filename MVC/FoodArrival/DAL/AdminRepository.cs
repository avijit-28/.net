using System;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class AdminRepository
    {
        public Admin Login(string email, string password)
        {
            var parameters = new[]
            {
                new SqlParameter("@Email", email ?? string.Empty),
                new SqlParameter("@PasswordHash", password ?? string.Empty)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Admin_Login", parameters);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Admin
                {
                    AdminId = Convert.ToInt32(row["AdminId"]),
                    AdminName = row["AdminName"].ToString(),
                    Email = row["Email"].ToString(),
                    IsActive = true
                };
            }
            return null;
        }

        public AdminDashboardViewModel GetDashboardSummary()
        {
            var vm = new AdminDashboardViewModel();
            var dt = DbHelper.ExecuteDataTable("dbo.sp_Admin_DashboardSummary");
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                vm.PendingOrders = Convert.ToInt32(row["PendingOrders"]);
                vm.AcceptedOrders = Convert.ToInt32(row["AcceptedOrders"]);
                vm.RejectedOrders = Convert.ToInt32(row["RejectedOrders"]);
                vm.AvailableFoodItems = Convert.ToInt32(row["AvailableFoodItems"]);
                vm.ActiveCustomers = Convert.ToInt32(row["ActiveCustomers"]);
            }
            return vm;
        }
    }
}
