using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class RestaurantRepository
    {
        public Restaurant Login(string email, string password)
        {
            string sql = @"
                SELECT RestaurantId, RestaurantName, Email, PhoneNumber, Address, Cuisine, Rating, ImageUrl, IsActive
                FROM dbo.Restaurants
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
                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public Restaurant GetById(int restaurantId)
        {
            string sql = @"
                SELECT RestaurantId, RestaurantName, Email, PhoneNumber, Address, Cuisine, Rating, ImageUrl, IsActive
                FROM dbo.Restaurants
                WHERE RestaurantId = @RestaurantId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Restaurant
                            {
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Cuisine = reader["Cuisine"] != DBNull.Value ? reader["Cuisine"].ToString() : null,
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public bool Update(Restaurant model, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.Restaurants
                SET RestaurantName = @RestaurantName,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address,
                    Cuisine = @Cuisine,
                    ImageUrl = @ImageUrl
                WHERE RestaurantId = @RestaurantId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantId", model.RestaurantId);
                    cmd.Parameters.AddWithValue("@RestaurantName", model.RestaurantName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cuisine", (object)model.Cuisine ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)model.ImageUrl ?? DBNull.Value);

                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        message = "Restaurant details updated successfully.";
                        return true;
                    }
                    message = "Restaurant not found.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<Order> GetOrders(int? restaurantId = null)
        {
            var list = new List<Order>();
            string sql = @"
                SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, c.Address, o.TotalAmount, 
                       o.OrderStatus, o.DeliveryStatus, o.OrderedAt, o.ProcessedAt, o.RejectionReason,
                       STUFF((
                           SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                           FROM dbo.OrderDetails od
                           INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                           WHERE od.OrderId = o.OrderId
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
                FROM dbo.Orders o
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                ORDER BY o.OrderedAt DESC";

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
                                ProcessedAt = reader["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ProcessedAt"]) : null,
                                RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : null,
                                Items = reader["Items"] != DBNull.Value ? reader["Items"].ToString() : null
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool UpdateDeliveryStatus(int orderId, string status)
        {
            string sql = @"UPDATE dbo.Orders SET DeliveryStatus = @Status WHERE OrderId = @OrderId";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }
    }
}
