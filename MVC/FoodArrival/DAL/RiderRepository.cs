using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class RiderRepository
    {
        public Rider Login(string email, string password)
        {
            string sql = @"
                SELECT RiderId, RiderName, Email, PhoneNumber, VehicleType, CurrentStatus, IsActive
                FROM dbo.Riders
                WHERE Email = @Email AND PasswordHash = @Password AND IsActive = 1";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Password", password ?? string.Empty);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rider
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                VehicleType = reader["VehicleType"] != DBNull.Value ? reader["VehicleType"].ToString() : "Motorbike",
                                CurrentStatus = reader["CurrentStatus"] != DBNull.Value ? reader["CurrentStatus"].ToString() : "Available",
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<Order> GetAvailableDeliveries()
        {
            var list = new List<Order>();
            string sql = @"
                SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, c.Address, o.TotalAmount, 
                       o.OrderStatus, o.DeliveryStatus, o.OrderedAt,
                       STUFF((
                           SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                           FROM dbo.OrderDetails od
                           INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                           WHERE od.OrderId = o.OrderId
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
                FROM dbo.Orders o
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                WHERE o.OrderStatus = 'Accepted' AND (o.RiderId IS NULL OR o.DeliveryStatus IN ('ReadyForPickup', 'PendingPickup'))
                ORDER BY o.OrderedAt ASC";

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
                            list.Add(new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                DeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : "Pending",
                                OrderedAt = Convert.ToDateTime(reader["OrderedAt"]),
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : null
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
            string sql = @"
                SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, c.Address, o.TotalAmount, 
                       o.OrderStatus, o.DeliveryStatus, o.OrderedAt, o.ProcessedAt,
                       STUFF((
                           SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                           FROM dbo.OrderDetails od
                           INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                           WHERE od.OrderId = o.OrderId
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
                FROM dbo.Orders o
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                WHERE o.RiderId = @RiderId
                ORDER BY o.OrderedAt DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                OrderStatus = reader["OrderStatus"].ToString(),
                                DeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : "Pending",
                                OrderedAt = Convert.ToDateTime(reader["OrderedAt"]),
                                ProcessedAt = reader["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ProcessedAt"]) : null,
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : null
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
            string sql = @"
                UPDATE dbo.Orders 
                SET RiderId = @RiderId, DeliveryStatus = 'OutForDelivery' 
                WHERE OrderId = @OrderId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        public bool CompleteDelivery(int orderId, int riderId)
        {
            string sql = @"
                UPDATE dbo.Orders 
                SET DeliveryStatus = 'Delivered' 
                WHERE OrderId = @OrderId AND RiderId = @RiderId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }
    }
}
