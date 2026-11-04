using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class RiderRepository
    {
        public bool Register(Rider rider, int restaurantId, out string message, out int riderId)
        {
            message = string.Empty;
            riderId = 0;

            // Check duplication
            string checkSql = @"
                SELECT 1 FROM dbo.Riders WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.Customers WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.Restaurants WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.RestaurantStaff WHERE LOWER(Email) = @Email OR LOWER(Username) = @Username
                UNION ALL
                SELECT 1 FROM dbo.Admins WHERE LOWER(Username) = @Username";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", rider.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Username", rider.Username.Trim().ToLower());
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            message = "Username or Email already registered in system.";
                            return false;
                        }
                    }
                }

                string insertSql = @"
                    INSERT INTO dbo.Riders 
                    (RiderName, Username, Email, PasswordHash, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive)
                    VALUES 
                    (@RiderName, @Username, @Email, @PasswordHash, @PhoneNumber, @VehicleType, @LicenseNumber, @LicenseDocumentUrl, 'Available', 1);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@RiderName", rider.RiderName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Username", rider.Username.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Email", rider.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", rider.PasswordHash ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", rider.PhoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@VehicleType", rider.VehicleType ?? "Motorbike");
                    cmd.Parameters.AddWithValue("@LicenseNumber", (object)rider.LicenseNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LicenseDocumentUrl", (object)rider.LicenseDocumentUrl ?? DBNull.Value);

                    conn.Open();
                    riderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Add affiliation with selected restaurant (pending verification)
                if (restaurantId > 0)
                {
                    AddAffiliation(riderId, restaurantId, out string _);
                }

                message = "Rider registered successfully! Your profile & driving documents are pending verification by the restaurant or admin.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool AddAffiliation(int riderId, int restaurantId, out string message)
        {
            message = string.Empty;
            string checkSql = "SELECT 1 FROM dbo.RiderRestaurantAffiliations WHERE RiderId = @RiderId AND RestaurantId = @RestaurantId";
            string insertSql = @"
                INSERT INTO dbo.RiderRestaurantAffiliations (RiderId, RestaurantId, IsVerified, AffiliatedAt)
                VALUES (@RiderId, @RestaurantId, 0, GETDATE());";

            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@RiderId", riderId);
                        cmdCheck.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        if (cmdCheck.ExecuteScalar() != null)
                        {
                            message = "You have already applied or registered with this restaurant.";
                            return false;
                        }
                    }

                    using (var cmdInsert = new SqlCommand(insertSql, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@RiderId", riderId);
                        cmdInsert.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        cmdInsert.ExecuteNonQuery();
                        message = "Affiliation request submitted. Pending verification by restaurant or admin.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public Rider Login(string loginId, string password)
        {
            string sql = @"
                SELECT RiderId, RiderName, Username, Email, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive
                FROM dbo.Riders
                WHERE (LOWER(Email) = @LoginId OR LOWER(Username) = @LoginId)
                  AND PasswordHash = @Password 
                  AND IsActive = 1";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LoginId", (loginId ?? string.Empty).Trim().ToLower());
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
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                VehicleType = reader["VehicleType"] != DBNull.Value ? reader["VehicleType"].ToString() : "Motorbike",
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
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

        public Rider GetById(int riderId)
        {
            string sql = @"
                SELECT RiderId, RiderName, Username, Email, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive, CreatedAt
                FROM dbo.Riders
                WHERE RiderId = @RiderId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RiderId", riderId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rider
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                VehicleType = reader["VehicleType"] != DBNull.Value ? reader["VehicleType"].ToString() : "Motorbike",
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                CurrentStatus = reader["CurrentStatus"] != DBNull.Value ? reader["CurrentStatus"].ToString() : "Available",
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<Rider> GetAll()
        {
            var list = new List<Rider>();
            string sql = @"
                SELECT RiderId, RiderName, Username, Email, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive, CreatedAt
                FROM dbo.Riders
                ORDER BY RiderId DESC";

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
                            list.Add(new Rider
                            {
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : null,
                                VehicleType = reader["VehicleType"] != DBNull.Value ? reader["VehicleType"].ToString() : "Motorbike",
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null,
                                CurrentStatus = reader["CurrentStatus"] != DBNull.Value ? reader["CurrentStatus"].ToString() : "Available",
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool Update(Rider model, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.Riders
                SET RiderName = @RiderName,
                    PhoneNumber = @PhoneNumber,
                    VehicleType = @VehicleType,
                    LicenseNumber = @LicenseNumber,
                    CurrentStatus = @CurrentStatus
                WHERE RiderId = @RiderId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RiderId", model.RiderId);
                    cmd.Parameters.AddWithValue("@RiderName", model.RiderName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", (object)model.VehicleType ?? "Motorbike");
                    cmd.Parameters.AddWithValue("@LicenseNumber", (object)model.LicenseNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrentStatus", (object)model.CurrentStatus ?? "Available");

                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        message = "Rider profile updated successfully.";
                        return true;
                    }
                    message = "Rider not found.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<RiderAffiliation> GetRiderAffiliations(int riderId)
        {
            var list = new List<RiderAffiliation>();
            string sql = @"
                SELECT a.AffiliationId, a.RiderId, a.RestaurantId, r.RestaurantName, a.IsVerified, 
                       a.VerifiedByRole, a.VerifiedAt, a.AffiliatedAt,
                       rd.RiderName, rd.PhoneNumber, rd.VehicleType, rd.LicenseNumber, rd.LicenseDocumentUrl
                FROM dbo.RiderRestaurantAffiliations a
                INNER JOIN dbo.Restaurants r ON a.RestaurantId = r.RestaurantId
                INNER JOIN dbo.Riders rd ON a.RiderId = rd.RiderId
                WHERE a.RiderId = @RiderId
                ORDER BY a.AffiliationId DESC";

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
                            list.Add(new RiderAffiliation
                            {
                                AffiliationId = Convert.ToInt32(reader["AffiliationId"]),
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                VerifiedByRole = reader["VerifiedByRole"] != DBNull.Value ? reader["VerifiedByRole"].ToString() : null,
                                VerifiedAt = reader["VerifiedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["VerifiedAt"]) : null,
                                AffiliatedAt = Convert.ToDateTime(reader["AffiliatedAt"]),
                                RiderName = reader["RiderName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<RiderAffiliation> GetRestaurantRiders(int? restaurantId = null, bool? onlyPending = null)
        {
            var list = new List<RiderAffiliation>();
            string sql = @"
                SELECT a.AffiliationId, a.RiderId, a.RestaurantId, r.RestaurantName, a.IsVerified, 
                       a.VerifiedByRole, a.VerifiedAt, a.AffiliatedAt,
                       rd.RiderName, rd.Username, rd.Email, rd.PhoneNumber, rd.VehicleType, rd.LicenseNumber, rd.LicenseDocumentUrl
                FROM dbo.RiderRestaurantAffiliations a
                INNER JOIN dbo.Restaurants r ON a.RestaurantId = r.RestaurantId
                INNER JOIN dbo.Riders rd ON a.RiderId = rd.RiderId
                WHERE (@RestId IS NULL OR a.RestaurantId = @RestId)
                  AND (@OnlyPending IS NULL OR a.IsVerified = 0)
                ORDER BY a.AffiliationId DESC";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestId", (object)restaurantId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OnlyPending", (object)onlyPending ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RiderAffiliation
                            {
                                AffiliationId = Convert.ToInt32(reader["AffiliationId"]),
                                RiderId = Convert.ToInt32(reader["RiderId"]),
                                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                                RestaurantName = reader["RestaurantName"].ToString(),
                                IsVerified = Convert.ToBoolean(reader["IsVerified"]),
                                VerifiedByRole = reader["VerifiedByRole"] != DBNull.Value ? reader["VerifiedByRole"].ToString() : null,
                                VerifiedAt = reader["VerifiedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["VerifiedAt"]) : null,
                                AffiliatedAt = Convert.ToDateTime(reader["AffiliatedAt"]),
                                RiderName = reader["RiderName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                LicenseNumber = reader["LicenseNumber"] != DBNull.Value ? reader["LicenseNumber"].ToString() : null,
                                LicenseDocumentUrl = reader["LicenseDocumentUrl"] != DBNull.Value ? reader["LicenseDocumentUrl"].ToString() : null
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public bool VerifyAffiliation(int affiliationId, string role, int userId, out string message)
        {
            message = string.Empty;
            string sql = @"
                UPDATE dbo.RiderRestaurantAffiliations
                SET IsVerified = 1, VerifiedByRole = @Role, VerifiedBy = @UserId, VerifiedAt = GETDATE()
                WHERE AffiliationId = @AffiliationId";

            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", role ?? "Admin");
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@AffiliationId", affiliationId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Rider affiliation verified and approved." : "Affiliation record not found.";
                    return ok;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool RejectAffiliation(int affiliationId, out string message)
        {
            message = string.Empty;
            string sql = @"DELETE FROM dbo.RiderRestaurantAffiliations WHERE AffiliationId = @AffiliationId";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AffiliationId", affiliationId);
                    conn.Open();
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    message = ok ? "Rider affiliation rejected." : "Affiliation record not found.";
                    return ok;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<Order> GetAvailableDeliveries()
        {
            var list = new List<Order>();
            string sql = @"
                SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, 
                       ISNULL(o.DeliveryAddress, c.Address) AS Address, o.TotalAmount, 
                       o.OrderStatus, o.DeliveryStatus, o.OrderedAt, 
                       ISNULL(r.RestaurantName, 'FoodReady Kitchen') AS RestaurantName,
                       STUFF((
                           SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                           FROM dbo.OrderDetails od
                           INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                           WHERE od.OrderId = o.OrderId
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
                FROM dbo.Orders o
                INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                LEFT JOIN dbo.Restaurants r ON o.RestaurantId = r.RestaurantId
                WHERE o.OrderStatus NOT IN ('Cancelled', 'Rejected')
                  AND o.DeliveryStatus != 'Delivered'
                  AND (
                      o.DeliveryStatus IN ('ReadyForPickup', 'PendingPickup', 'Pending', 'Accepted')
                      OR EXISTS (
                          SELECT 1 FROM dbo.OrderSubOrders oso 
                          WHERE oso.OrderId = o.OrderId 
                            AND oso.PreparationStatus = 'ReadyForPickup'
                      )
                  )
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
                SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, 
                       ISNULL(o.DeliveryAddress, c.Address) AS Address, o.TotalAmount, 
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
                WHERE (
                    o.RiderId = @RiderId 
                    OR EXISTS (
                        SELECT 1 FROM dbo.OrderSubOrders oso 
                        WHERE oso.OrderId = o.OrderId AND oso.RiderId = @RiderId
                    )
                )
                AND o.OrderStatus NOT IN ('Cancelled', 'Rejected')
                AND (o.DeliveryStatus != 'Delivered' OR o.OrderedAt >= DATEADD(day, -1, GETDATE()))
                ORDER BY CASE WHEN o.DeliveryStatus = 'Delivered' THEN 1 ELSE 0 END, o.OrderedAt DESC";

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
                WHERE OrderId = @OrderId;

                UPDATE dbo.OrderSubOrders
                SET RiderId = @RiderId, DeliveryStatus = 'OutForDelivery'
                WHERE OrderId = @OrderId;

                UPDATE dbo.Riders SET CurrentStatus = 'On Delivery' WHERE RiderId = @RiderId;";

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

        public bool CompleteDelivery(int orderId, int riderId, string enteredOtp, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(enteredOtp))
            {
                errorMessage = "Please enter the 6-digit delivery OTP provided by the customer.";
                return false;
            }

            enteredOtp = enteredOtp.Trim();
            if (enteredOtp.Length != 6)
            {
                errorMessage = "Delivery OTP must be exactly 6 digits.";
                return false;
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // 1. Verify OTP against order
                    string checkSql = "SELECT DeliveryOtp, DeliveryStatus FROM dbo.Orders WHERE OrderId = @OrderId;";
                    string expectedOtp = null;
                    string currentDeliveryStatus = null;

                    using (var checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@OrderId", orderId);
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                expectedOtp = reader["DeliveryOtp"] != DBNull.Value ? reader["DeliveryOtp"].ToString().Trim() : null;
                                currentDeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString().Trim() : null;
                            }
                            else
                            {
                                errorMessage = "Order not found.";
                                return false;
                            }
                        }
                    }

                    if (currentDeliveryStatus == "Delivered")
                    {
                        errorMessage = "This order has already been marked as delivered.";
                        return false;
                    }

                    if (string.IsNullOrEmpty(expectedOtp))
                    {
                        errorMessage = "No delivery OTP was set for this order. Please contact support.";
                        return false;
                    }

                    if (!string.Equals(expectedOtp, enteredOtp, StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = "Invalid Delivery OTP! The code you entered does not match the customer's PIN. Please verify with the customer.";
                        return false;
                    }

                    // 2. OTP is verified! Mark delivered.
                    string sql = @"
                        UPDATE dbo.Orders 
                        SET DeliveryStatus = 'Delivered', OrderStatus = 'Accepted', ProcessedAt = GETDATE() 
                        WHERE OrderId = @OrderId;

                        UPDATE dbo.OrderSubOrders
                        SET DeliveryStatus = 'Delivered', DeliveredAt = GETDATE()
                        WHERE OrderId = @OrderId;

                        UPDATE dbo.Riders SET CurrentStatus = 'Available' WHERE RiderId = @RiderId;
                        
                        INSERT INTO dbo.OrderStatusHistory (OrderId, Status, ChangedByUserId, ChangedByRole, Message, CreatedAt)
                        VALUES (@OrderId, 'Delivered', @RiderId, 'Rider', 'Handover confirmed via customer 6-digit OTP verification.', GETDATE());";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@RiderId", riderId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Database error verifying OTP: " + ex.Message;
                return false;
            }
        }

        public bool CompleteDelivery(int orderId, int riderId)
        {
            string msg;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("SELECT DeliveryOtp FROM dbo.Orders WHERE OrderId = @OrderId", conn))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    conn.Open();
                    var obj = cmd.ExecuteScalar();
                    string otp = obj != null && obj != DBNull.Value ? obj.ToString() : "";
                    return CompleteDelivery(orderId, riderId, otp, out msg);
                }
            }
            catch { return false; }
        }

        public bool ResetPassword(int riderId, string newPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.Riders SET PasswordHash = @Password WHERE RiderId = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Id", riderId);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        message = "Password reset successfully.";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}