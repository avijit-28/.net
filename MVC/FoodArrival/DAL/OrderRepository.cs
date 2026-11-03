using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class OrderRepository
    {
        public bool CreateOrder(int customerId, List<CartItem> items, out string message, out int orderId)
        {
            return CreateMasterOrder(customerId, items, null, "UPI", out message, out orderId);
        }

        public bool CreateMasterOrder(int customerId, List<CartItem> items, string deliveryAddress, string paymentMethod, out string message, out int orderId)
        {
            message = string.Empty;
            orderId = 0;

            if (items == null || items.Count == 0)
            {
                message = "Order must contain at least one item.";
                return false;
            }

            decimal subTotal = 0;
            foreach (var item in items) subTotal += item.SubTotal;
            decimal deliveryFee = 40.00m;
            decimal platformFee = 15.00m;
            decimal totalAmount = subTotal + deliveryFee + platformFee;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Master Order
                        string insertMasterSql = @"
                            INSERT INTO dbo.Orders 
                            (CustomerId, TotalAmount, OrderStatus, OrderedAt, DeliveryFee, DeliveryAddress, PaymentStatus, PaymentMethod, DeliveryStatus, EstimatedDeliveryTime)
                            VALUES 
                            (@CustomerId, @TotalAmount, 'Pending', GETDATE(), @DeliveryFee, @DeliveryAddress, 'Success', @PaymentMethod, 'Pending', DATEADD(MINUTE, 35, GETDATE()));
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int newOrderId;
                        using (var cmd = new SqlCommand(insertMasterSql, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            cmd.Parameters.AddWithValue("@DeliveryFee", deliveryFee);
                            cmd.Parameters.AddWithValue("@DeliveryAddress", (object)deliveryAddress ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentMethod", (object)paymentMethod ?? "UPI");
                            newOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        orderId = newOrderId;
                        string orderNumber = "FR" + (100000 + newOrderId);

                        // Update OrderNumber
                        using (var cmd = new SqlCommand("UPDATE dbo.Orders SET OrderNumber = @OrderNum WHERE OrderId = @OrderId", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderNum", orderNumber);
                            cmd.Parameters.AddWithValue("@OrderId", newOrderId);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Group items by RestaurantId and insert OrderSubOrders
                        var restaurantGroups = new Dictionary<int, List<CartItem>>();
                        foreach (var itm in items)
                        {
                            int rId = itm.RestaurantId > 0 ? itm.RestaurantId : 1;
                            if (!restaurantGroups.ContainsKey(rId))
                                restaurantGroups[rId] = new List<CartItem>();
                            restaurantGroups[rId].Add(itm);
                        }

                        foreach (var kvp in restaurantGroups)
                        {
                            int restId = kvp.Key;
                            var restItems = kvp.Value;
                            decimal restSubTotal = 0;
                            foreach (var itm in restItems) restSubTotal += itm.SubTotal;

                            // Fetch restaurant prep minutes
                            int prepMinutes = 15;
                            using (var cmdPrep = new SqlCommand("SELECT ISNULL(EstimatedPrepMinutes, 15) FROM dbo.Restaurants WHERE RestaurantId = @RestId", conn, tran))
                            {
                                cmdPrep.Parameters.AddWithValue("@RestId", restId);
                                var scalar = cmdPrep.ExecuteScalar();
                                if (scalar != null && scalar != DBNull.Value) prepMinutes = Convert.ToInt32(scalar);
                            }

                            int subOrderId;
                            string insertSubSql = @"
                                INSERT INTO dbo.OrderSubOrders 
                                (OrderId, RestaurantId, SubTotal, PreparationStatus, EstimatedReadyAt, DeliveryStatus, PickupSequence, CreatedAt)
                                VALUES 
                                (@OrderId, @RestaurantId, @SubTotal, 'Confirmed', DATEADD(MINUTE, @PrepMin, GETDATE()), 'Pending', 1, GETDATE());
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (var cmdSub = new SqlCommand(insertSubSql, conn, tran))
                            {
                                cmdSub.Parameters.AddWithValue("@OrderId", newOrderId);
                                cmdSub.Parameters.AddWithValue("@RestaurantId", restId);
                                cmdSub.Parameters.AddWithValue("@SubTotal", restSubTotal);
                                cmdSub.Parameters.AddWithValue("@PrepMin", prepMinutes);
                                subOrderId = Convert.ToInt32(cmdSub.ExecuteScalar());
                            }

                            // 3. Insert OrderDetails for this restaurant
                            foreach (var item in restItems)
                            {
                                string insertDetailSql = @"
                                    INSERT INTO dbo.OrderDetails 
                                    (OrderId, SubOrderId, FoodItemId, Quantity, UnitPrice, SubTotal)
                                    VALUES 
                                    (@OrderId, @SubOrderId, @FoodItemId, @Quantity, @UnitPrice, @SubTotal);";

                                using (var cmdDetail = new SqlCommand(insertDetailSql, conn, tran))
                                {
                                    cmdDetail.Parameters.AddWithValue("@OrderId", newOrderId);
                                    cmdDetail.Parameters.AddWithValue("@SubOrderId", subOrderId);
                                    cmdDetail.Parameters.AddWithValue("@FoodItemId", item.FoodItemId);
                                    cmdDetail.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    cmdDetail.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                    cmdDetail.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                                    cmdDetail.ExecuteNonQuery();
                                }
                            }
                        }

                        // 4. Record initial Timeline event
                        string historySql = @"
                            INSERT INTO dbo.OrderStatusHistory (OrderId, Status, ChangedByRole, Message, CreatedAt)
                            VALUES (@OrderId, 'Confirmed', 'Customer', @Message, GETDATE());";
                        using (var cmdHist = new SqlCommand(historySql, conn, tran))
                        {
                            cmdHist.Parameters.AddWithValue("@OrderId", newOrderId);
                            cmdHist.Parameters.AddWithValue("@Message", "Master order #" + orderNumber + " placed across " + restaurantGroups.Count + " kitchen partner(s).");
                            cmdHist.ExecuteNonQuery();
                        }

                        // 5. Record Transaction
                        string txnSql = @"
                            INSERT INTO dbo.Transactions 
                            (OrderId, TransactionReference, Amount, PaymentMethod, PaymentStatus, RestaurantShare, PlatformFee, DeliveryFee, CreatedAt)
                            VALUES 
                            (@OrderId, @Ref, @Amount, @Method, 'Success', @RestShare, @PlatformFee, @DeliveryFee, GETDATE());";
                        using (var cmdTxn = new SqlCommand(txnSql, conn, tran))
                        {
                            cmdTxn.Parameters.AddWithValue("@OrderId", newOrderId);
                            cmdTxn.Parameters.AddWithValue("@Ref", "TXN-" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());
                            cmdTxn.Parameters.AddWithValue("@Amount", totalAmount);
                            cmdTxn.Parameters.AddWithValue("@Method", (object)paymentMethod ?? "UPI");
                            cmdTxn.Parameters.AddWithValue("@RestShare", subTotal * 0.90m);
                            cmdTxn.Parameters.AddWithValue("@PlatformFee", platformFee + (subTotal * 0.10m));
                            cmdTxn.Parameters.AddWithValue("@DeliveryFee", deliveryFee);
                            cmdTxn.ExecuteNonQuery();
                        }

                        tran.Commit();
                        message = "Order placed successfully!";
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        message = "Transaction rolled back: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        public List<Order> GetByCustomer(int customerId)
        {
            var list = new List<Order>();
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetByCustomer", parameters);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Order
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    OrderStatus = row["OrderStatus"].ToString(),
                    OrderedAt = Convert.ToDateTime(row["OrderedAt"]),
                    ProcessedAt = row["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ProcessedAt"]) : null
                });
            }
            return list;
        }

        public Order GetDetails(int orderId)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderId", orderId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetDetails", parameters);
            if (dt.Rows.Count == 0) return null;

            var firstRow = dt.Rows[0];
            var order = new Order
            {
                OrderId = Convert.ToInt32(firstRow["OrderId"]),
                CustomerId = Convert.ToInt32(firstRow["CustomerId"]),
                CustomerName = firstRow["CustomerName"].ToString(),
                Email = firstRow["Email"].ToString(),
                PhoneNumber = firstRow["PhoneNumber"] != DBNull.Value ? firstRow["PhoneNumber"].ToString() : null,
                Address = firstRow["Address"] != DBNull.Value ? firstRow["Address"].ToString() : null,
                TotalAmount = Convert.ToDecimal(firstRow["TotalAmount"]),
                OrderStatus = firstRow["OrderStatus"].ToString(),
                OrderedAt = Convert.ToDateTime(firstRow["OrderedAt"]),
                ProcessedAt = firstRow["ProcessedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(firstRow["ProcessedAt"]) : null,
                RejectionReason = firstRow["RejectionReason"] != DBNull.Value ? firstRow["RejectionReason"].ToString() : null
            };

            foreach (DataRow row in dt.Rows)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    FoodItemId = Convert.ToInt32(row["FoodItemId"]),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    SubTotal = Convert.ToDecimal(row["SubTotal"])
                });
            }

            return order;
        }

        private List<Order> MapOrderList(DataTable dt)
        {
            var list = new List<Order>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Order
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    Email = row["Email"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    OrderStatus = row["OrderStatus"].ToString(),
                    OrderedAt = Convert.ToDateTime(row["OrderedAt"]),
                    ProcessedAt = row.Table.Columns.Contains("ProcessedAt") && row["ProcessedAt"] != DBNull.Value
                        ? (DateTime?)Convert.ToDateTime(row["ProcessedAt"])
                        : null,
                    RejectionReason = row.Table.Columns.Contains("RejectionReason") && row["RejectionReason"] != DBNull.Value
                        ? row["RejectionReason"].ToString()
                        : null,
                    Items = row.Table.Columns.Contains("Items") && row["Items"] != DBNull.Value
                        ? row["Items"].ToString()
                        : string.Empty
                });
            }
            return list;
        }

        public List<Order> GetPendingOrders()
        {
            // Auto reject overdue orders first
            AutoRejectOrders();

            DataTable dt;
            try
            {
                dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetPending");
            }
            catch
            {
                dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetTodayPending");
            }
            return MapOrderList(dt);
        }

        public List<Order> GetAcceptedOrders()
        {
            DataTable dt;
            try
            {
                dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetAccepted");
            }
            catch
            {
                dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetTodayAccepted");
            }
            return MapOrderList(dt);
        }

        public List<Order> GetRejectedOrders()
        {
            DataTable dt;
            try
            {
                dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetRejected");
            }
            catch
            {
                dt = DbHelper.ExecuteDataTable("dbo.sp_Order_GetTodayRejected");
            }
            return MapOrderList(dt);
        }

        public bool AcceptOrder(int orderId, int adminId, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@AdminId", adminId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Order_Accept", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public bool RejectOrder(int orderId, int adminId, string reason, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@AdminId", adminId),
                new SqlParameter("@RejectionReason", (object)reason ?? DBNull.Value)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Order_Reject", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public int AutoRejectOrders()
        {
            try
            {
                var dt = DbHelper.ExecuteDataTable("dbo.sp_Order_AutoReject");
                if (dt.Rows.Count > 0 && dt.Rows[0]["RejectedOrders"] != DBNull.Value)
                {
                    return Convert.ToInt32(dt.Rows[0]["RejectedOrders"]);
                }
            }
            catch
            {
                // Ignore if procedure call fails
            }
            return 0;
        }
    }
}
