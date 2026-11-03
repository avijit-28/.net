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
            message = string.Empty;
            orderId = 0;

            if (items == null || items.Count == 0)
            {
                message = "Order must contain at least one item.";
                return false;
            }

            var tvpParam = DbHelper.CreateOrderItemTableParam("@Items", items);
            var custParam = new SqlParameter("@CustomerId", customerId);

            try
            {
                var dt = DbHelper.ExecuteDataTable("dbo.sp_Order_CreateMultiple", custParam, tvpParam);
                if (dt.Rows.Count > 0)
                {
                    int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                    message = dt.Rows[0]["Message"].ToString();
                    if (result == 1)
                    {
                        orderId = Convert.ToInt32(dt.Rows[0]["OrderId"]);
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }

            return false;
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
