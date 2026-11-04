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
                        string deliveryOtp = new Random().Next(100000, 999999).ToString();

                        // 1. Insert Master Order via Stored Procedure
                        int newOrderId;
                        string orderNumber;
                        using (var cmd = new SqlCommand("dbo.sp_Order_CreateMaster", conn, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            cmd.Parameters.AddWithValue("@DeliveryFee", deliveryFee);
                            cmd.Parameters.AddWithValue("@DeliveryAddress", (object)deliveryAddress ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentMethod", (object)paymentMethod ?? "UPI");
                            cmd.Parameters.AddWithValue("@DeliveryOtp", deliveryOtp);

                            var outId = new SqlParameter("@NewOrderId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var outNum = new SqlParameter("@OrderNumber", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(outId);
                            cmd.Parameters.Add(outNum);

                            cmd.ExecuteNonQuery();
                            newOrderId = Convert.ToInt32(outId.Value);
                            orderNumber = outNum.Value.ToString();
                        }

                        orderId = newOrderId;

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

                            int prepMinutes = 15;
                            using (var cmdPrep = new SqlCommand("dbo.sp_Restaurant_GetById", conn, tran))
                            {
                                cmdPrep.CommandType = CommandType.StoredProcedure;
                                cmdPrep.Parameters.AddWithValue("@RestaurantId", restId);
                                using (var rdr = cmdPrep.ExecuteReader())
                                {
                                    // Default prep minutes is 15
                                }
                            }

                            int subOrderId;
                            using (var cmdSub = new SqlCommand("dbo.sp_Order_CreateSubOrder", conn, tran))
                            {
                                cmdSub.CommandType = CommandType.StoredProcedure;
                                cmdSub.Parameters.AddWithValue("@OrderId", newOrderId);
                                cmdSub.Parameters.AddWithValue("@RestaurantId", restId);
                                cmdSub.Parameters.AddWithValue("@SubTotal", restSubTotal);
                                cmdSub.Parameters.AddWithValue("@PrepMinutes", prepMinutes);
                                cmdSub.Parameters.AddWithValue("@DeliveryOtp", deliveryOtp);

                                var outSubId = new SqlParameter("@SubOrderId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                                cmdSub.Parameters.Add(outSubId);

                                cmdSub.ExecuteNonQuery();
                                subOrderId = Convert.ToInt32(outSubId.Value);
                            }

                            // 3. Insert OrderDetails for this restaurant via Stored Procedure
                            foreach (var item in restItems)
                            {
                                using (var cmdDetail = new SqlCommand("dbo.sp_Order_CreateDetail", conn, tran))
                                {
                                    cmdDetail.CommandType = CommandType.StoredProcedure;
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

                        // 4. Record initial Timeline event via Stored Procedure
                        using (var cmdHist = new SqlCommand("dbo.sp_OrderStatusHistory_Add", conn, tran))
                        {
                            cmdHist.CommandType = CommandType.StoredProcedure;
                            cmdHist.Parameters.AddWithValue("@OrderId", newOrderId);
                            cmdHist.Parameters.AddWithValue("@SubOrderId", DBNull.Value);
                            cmdHist.Parameters.AddWithValue("@Status", "Placed");
                            cmdHist.Parameters.AddWithValue("@ChangedByUserId", customerId);
                            cmdHist.Parameters.AddWithValue("@ChangedByRole", "Customer");
                            cmdHist.Parameters.AddWithValue("@Message", "Master order #" + orderNumber + " placed across " + restaurantGroups.Count + " kitchen partner(s).");
                            cmdHist.ExecuteNonQuery();
                        }

                        // 5. Record Transaction via Stored Procedure
                        using (var cmdTxn = new SqlCommand("dbo.sp_Transaction_Insert", conn, tran))
                        {
                            cmdTxn.CommandType = CommandType.StoredProcedure;
                            cmdTxn.Parameters.AddWithValue("@OrderId", newOrderId);
                            cmdTxn.Parameters.AddWithValue("@TransactionReference", "TXN-" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());
                            cmdTxn.Parameters.AddWithValue("@Amount", totalAmount);
                            cmdTxn.Parameters.AddWithValue("@PaymentMethod", (object)paymentMethod ?? "UPI");
                            cmdTxn.Parameters.AddWithValue("@PaymentStatus", "Success");
                            cmdTxn.Parameters.AddWithValue("@RestaurantShare", subTotal * 0.90m);
                            cmdTxn.Parameters.AddWithValue("@PlatformFee", platformFee + (subTotal * 0.10m));
                            cmdTxn.Parameters.AddWithValue("@DeliveryFee", deliveryFee);
                            cmdTxn.Parameters.AddWithValue("@GatewayResponse", DBNull.Value);
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

        public Order GetById(int orderId)
        {
            return GetDetails(orderId);
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
                DeliveryStatus = firstRow.Table.Columns.Contains("DeliveryStatus") && firstRow["DeliveryStatus"] != DBNull.Value ? firstRow["DeliveryStatus"].ToString() : "Pending",
                DeliveryOtp = firstRow.Table.Columns.Contains("DeliveryOtp") && firstRow["DeliveryOtp"] != DBNull.Value ? firstRow["DeliveryOtp"].ToString() : null,
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
