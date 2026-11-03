using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;

namespace FoodArrival.Models
{
    public class FoodRepository
    {
        private readonly string _connectionString;

        public FoodRepository()
        {
            var setting = ConfigurationManager.ConnectionStrings["FoodDeliveryDB"];
            if (setting == null || String.IsNullOrWhiteSpace(setting.ConnectionString))
                throw new ConfigurationErrorsException("Add a connection string named 'FoodDeliveryDB' to Web.config.");
            _connectionString = setting.ConnectionString;
        }

        private SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private static SqlCommand Procedure(SqlConnection connection, string name)
        {
            return new SqlCommand(name, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = 30 };
        }

        private static string Text(SqlDataReader reader, string name)
        {
            var value = reader[name];
            return value == DBNull.Value ? null : Convert.ToString(value);
        }

        private static RestaurantCard ReadRestaurant(SqlDataReader reader)
        {
            return new RestaurantCard
            {
                RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                Name = Text(reader, "Name"),
                Description = Text(reader, "Description"),
                Cuisine = Text(reader, "Cuisine"),
                City = Text(reader, "City"),
                AddressLine = Text(reader, "AddressLine"),
                ImageUrl = Text(reader, "ImageUrl"),
                AverageRating = reader["AverageRating"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["AverageRating"]),
                ReviewCount = Convert.ToInt32(reader["ReviewCount"])
            };
        }

        public IList<RestaurantCard> SearchRestaurants(string city, string search)
        {
            var result = new List<RestaurantCard>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Restaurant_Search"))
            {
                command.Parameters.Add("@City", SqlDbType.NVarChar, 100).Value = (object)city ?? DBNull.Value;
                command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = (object)search ?? DBNull.Value;
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) result.Add(ReadRestaurant(reader));
            }
            return result;
        }

        public RestaurantPageViewModel GetRestaurant(int id, string category, bool vegetarianOnly)
        {
            var model = new RestaurantPageViewModel { Menu = new List<MenuItem>(), Category = category, VegetarianOnly = vegetarianOnly };
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Restaurant_Search"))
            {
                command.Parameters.Add("@City", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["RestaurantId"]) == id) { model.Restaurant = ReadRestaurant(reader); break; }
                    }
            }
            if (model.Restaurant == null) return null;
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Menu_List"))
            {
                command.Parameters.Add("@RestaurantId", SqlDbType.Int).Value = id;
                command.Parameters.Add("@Category", SqlDbType.NVarChar, 80).Value = (object)category ?? DBNull.Value;
                command.Parameters.Add("@VegetarianOnly", SqlDbType.Bit).Value = vegetarianOnly;
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) model.Menu.Add(new MenuItem
                    {
                        MenuItemId = Convert.ToInt32(reader["MenuItemId"]), RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                        Name = Text(reader, "Name"), Description = Text(reader, "Description"), Category = Text(reader, "Category"),
                        Price = Convert.ToDecimal(reader["Price"]), ImageUrl = Text(reader, "ImageUrl"),
                        IsVegetarian = Convert.ToBoolean(reader["IsVegetarian"]), IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                    });
            }
            return model;
        }

        public int SaveMenuItem(int actorId, int restaurantId, MenuEditViewModel model)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_MenuItem_Save"))
            {
                command.Parameters.Add("@ActorUserId", SqlDbType.Int).Value = actorId;
                command.Parameters.Add("@RestaurantId", SqlDbType.Int).Value = restaurantId;
                command.Parameters.Add("@MenuItemId", SqlDbType.Int).Value = (object)model.MenuItemId ?? DBNull.Value;
                command.Parameters.Add("@Name", SqlDbType.NVarChar, 160).Value = model.Name;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 600).Value = (object)model.Description ?? DBNull.Value;
                command.Parameters.Add("@Category", SqlDbType.NVarChar, 80).Value = (object)model.Category ?? DBNull.Value;
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = model.Price;
                command.Parameters.Add("@ImageUrl", SqlDbType.NVarChar, 500).Value = DBNull.Value;
                command.Parameters.Add("@IsVegetarian", SqlDbType.Bit).Value = model.IsVegetarian;
                command.Parameters.Add("@IsAvailable", SqlDbType.Bit).Value = model.IsAvailable;
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetMenuAvailability(int actorId, int itemId, bool isAvailable)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_MenuItem_SetAvailability"))
            {
                command.Parameters.Add("@ActorUserId", SqlDbType.Int).Value = actorId;
                command.Parameters.Add("@MenuItemId", SqlDbType.Int).Value = itemId;
                command.Parameters.Add("@IsAvailable", SqlDbType.Bit).Value = isAvailable;
                command.ExecuteNonQuery();
            }
        }

        public long PlaceOrder(int customerId, int restaurantId, CheckoutViewModel checkout, IList<CartLine> lines)
        {
            var items = lines.Select(x => new { menuItemId = x.MenuItemId, quantity = x.Quantity }).ToArray();
            var json = new JavaScriptSerializer().Serialize(items);
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Order_Place"))
            {
                command.Parameters.Add("@CustomerUserId", SqlDbType.Int).Value = customerId;
                command.Parameters.Add("@RestaurantId", SqlDbType.Int).Value = restaurantId;
                command.Parameters.Add("@AddressId", SqlDbType.Int).Value = DBNull.Value;
                command.Parameters.Add("@DeliveryRecipient", SqlDbType.NVarChar, 120).Value = checkout.RecipientName;
                command.Parameters.Add("@DeliveryPhone", SqlDbType.NVarChar, 30).Value = checkout.Phone;
                command.Parameters.Add("@DeliveryAddressLine", SqlDbType.NVarChar, 250).Value = checkout.AddressLine;
                command.Parameters.Add("@DeliveryAddressLine2", SqlDbType.NVarChar, 250).Value = (object)checkout.AddressLine2 ?? DBNull.Value;
                command.Parameters.Add("@DeliveryCity", SqlDbType.NVarChar, 100).Value = checkout.City;
                command.Parameters.Add("@DeliveryPostalCode", SqlDbType.NVarChar, 20).Value = (object)checkout.PostalCode ?? DBNull.Value;
                command.Parameters.Add("@DeliveryInstructions", SqlDbType.NVarChar, 500).Value = (object)checkout.Instructions ?? DBNull.Value;
                command.Parameters.Add("@DeliveryFee", SqlDbType.Decimal).Value = 0m;
                command.Parameters.Add("@TaxAmount", SqlDbType.Decimal).Value = 0m;
                command.Parameters.Add("@PaymentMethod", SqlDbType.NVarChar, 30).Value = checkout.PaymentMethod;
                command.Parameters.Add("@ItemsJson", SqlDbType.NVarChar, -1).Value = json;
                using (var reader = command.ExecuteReader())
                    return reader.Read() ? Convert.ToInt64(reader["OrderId"]) : 0;
            }
        }

        public IList<OrderSummary> GetCustomerOrders(int customerId)
        {
            var orders = new List<OrderSummary>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Order_CustomerList"))
            {
                command.Parameters.Add("@CustomerUserId", SqlDbType.Int).Value = customerId;
                using (var reader = command.ExecuteReader()) while (reader.Read()) orders.Add(ReadOrder(reader));
            }
            return orders;
        }

        private static OrderSummary ReadOrder(SqlDataReader reader)
        {
            return new OrderSummary
            {
                OrderId = Convert.ToInt64(reader["OrderId"]), OrderNumber = Text(reader, "OrderNumber"),
                RestaurantId = Convert.ToInt32(reader["RestaurantId"]), RestaurantName = Text(reader, "RestaurantName"),
                Subtotal = Convert.ToDecimal(reader["Subtotal"]), DeliveryFee = Convert.ToDecimal(reader["DeliveryFee"]),
                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]), TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                OrderStatus = Text(reader, "OrderStatus"), CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                DeliveryStatus = Text(reader, "DeliveryStatus")
            };
        }

        public OrderDetailsViewModel GetCustomerOrder(int customerId, long orderId)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Order_CustomerDetails"))
            {
                command.Parameters.Add("@CustomerUserId", SqlDbType.Int).Value = customerId;
                command.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = orderId;
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    var model = new OrderDetailsViewModel
                    {
                        Order = new OrderSummary
                        {
                            OrderId = Convert.ToInt64(reader["OrderId"]), OrderNumber = Text(reader, "OrderNumber"),
                            RestaurantId = Convert.ToInt32(reader["RestaurantId"]), RestaurantName = Text(reader, "RestaurantName"),
                            Subtotal = Convert.ToDecimal(reader["Subtotal"]), DeliveryFee = Convert.ToDecimal(reader["DeliveryFee"]),
                            TaxAmount = Convert.ToDecimal(reader["TaxAmount"]), TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                            OrderStatus = Text(reader, "OrderStatus"), CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        },
                        RecipientName = Text(reader, "DeliveryRecipient"), Phone = Text(reader, "DeliveryPhone"),
                        Address = Text(reader, "DeliveryAddressLine"), City = Text(reader, "DeliveryCity"),
                        DeliveryStatus = Text(reader, "DeliveryStatus"), PaymentMethod = Text(reader, "PaymentMethod"),
                        PaymentStatus = Text(reader, "PaymentStatus"), Items = new List<OrderLine>()
                    };
                    if (reader.NextResult()) while (reader.Read()) model.Items.Add(new OrderLine
                    {
                        MenuItemId = Convert.ToInt32(reader["MenuItemId"]), ItemName = Text(reader, "ItemName"),
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"]), Quantity = Convert.ToInt32(reader["Quantity"]),
                        LineTotal = Convert.ToDecimal(reader["LineTotal"])
                    });
                    return model;
                }
            }
        }

        public IList<KeyValuePair<int, string>> GetOwnedRestaurants(int ownerId)
        {
            var result = new List<KeyValuePair<int, string>>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Restaurant_OwnedList"))
            {
                command.Parameters.Add("@OwnerUserId", SqlDbType.Int).Value = ownerId;
                using (var reader = command.ExecuteReader()) while (reader.Read())
                    result.Add(new KeyValuePair<int, string>(Convert.ToInt32(reader["RestaurantId"]), Text(reader, "Name")));
            }
            return result;
        }

        public IList<MenuItem> GetOwnedMenu(int ownerId, int restaurantId)
        {
            var result = new List<MenuItem>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Menu_OwnerList"))
            {
                command.Parameters.Add("@ActorUserId", SqlDbType.Int).Value = ownerId;
                command.Parameters.Add("@RestaurantId", SqlDbType.Int).Value = restaurantId;
                using (var reader = command.ExecuteReader()) while (reader.Read()) result.Add(new MenuItem
                {
                    MenuItemId = Convert.ToInt32(reader["MenuItemId"]), RestaurantId = restaurantId,
                    Name = Text(reader, "Name"), Description = Text(reader, "Description"), Category = Text(reader, "Category"),
                    Price = Convert.ToDecimal(reader["Price"]), IsVegetarian = Convert.ToBoolean(reader["IsVegetarian"]), IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                });
            }
            return result;
        }

        public IList<OrderSummary> GetRestaurantOrders(int ownerId, int restaurantId)
        {
            var result = new List<OrderSummary>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Order_RestaurantList"))
            {
                command.Parameters.Add("@ActorUserId", SqlDbType.Int).Value = ownerId;
                command.Parameters.Add("@RestaurantId", SqlDbType.Int).Value = restaurantId;
                command.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                using (var reader = command.ExecuteReader()) while (reader.Read()) result.Add(new OrderSummary
                {
                    OrderId = Convert.ToInt64(reader["OrderId"]), OrderNumber = Text(reader, "OrderNumber"),
                    RestaurantId = restaurantId, RestaurantName = null, Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                    DeliveryFee = Convert.ToDecimal(reader["DeliveryFee"]), TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]), OrderStatus = Text(reader, "OrderStatus"),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }
            return result;
        }

        public void SetRestaurantOrderStatus(int actorId, long orderId, string status)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Order_RestaurantSetStatus"))
            {
                command.Parameters.Add("@ActorUserId", SqlDbType.Int).Value = actorId;
                command.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = orderId;
                command.Parameters.Add("@NewStatus", SqlDbType.NVarChar, 20).Value = status;
                command.ExecuteNonQuery();
            }
        }

        public IList<DeliveryAssignmentViewModel> GetDeliveries(int agentId, bool includeCompleted)
        {
            var result = new List<DeliveryAssignmentViewModel>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Delivery_AgentList"))
            {
                command.Parameters.Add("@AgentUserId", SqlDbType.Int).Value = agentId;
                command.Parameters.Add("@IncludeCompleted", SqlDbType.Bit).Value = includeCompleted;
                using (var reader = command.ExecuteReader()) while (reader.Read()) result.Add(new DeliveryAssignmentViewModel
                {
                    AssignmentId = Convert.ToInt64(reader["AssignmentId"]), OrderId = Convert.ToInt64(reader["OrderId"]),
                    OrderNumber = Text(reader, "OrderNumber"), Status = Text(reader, "Status"), RestaurantName = Text(reader, "RestaurantName"),
                    PickupAddress = Text(reader, "PickupAddress"), Recipient = Text(reader, "DeliveryRecipient"), Phone = Text(reader, "DeliveryPhone"),
                    DeliveryAddress = Text(reader, "DeliveryAddressLine"), City = Text(reader, "DeliveryCity"), PostalCode = Text(reader, "DeliveryPostalCode")
                });
            }
            return result;
        }

        public void SetDeliveryStatus(int agentId, long orderId, string status)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Delivery_SetStatus"))
            {
                command.Parameters.Add("@AgentUserId", SqlDbType.Int).Value = agentId;
                command.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = orderId;
                command.Parameters.Add("@NewStatus", SqlDbType.NVarChar, 20).Value = status;
                command.ExecuteNonQuery();
            }
        }

        public IList<ReadyOrderViewModel> GetReadyOrders()
        {
            var result = new List<ReadyOrderViewModel>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Admin_ReadyOrders"))
            using (var reader = command.ExecuteReader()) while (reader.Read()) result.Add(new ReadyOrderViewModel
            {
                OrderId = Convert.ToInt64(reader["OrderId"]), OrderNumber = Text(reader, "OrderNumber"),
                RestaurantName = Text(reader, "RestaurantName"), City = Text(reader, "DeliveryCity"),
                AgentUserId = reader["AgentUserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["AgentUserId"])
            });
            return result;
        }

        public IList<KeyValuePair<int, string>> GetActiveAgents()
        {
            var result = new List<KeyValuePair<int, string>>();
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Admin_AgentList"))
            using (var reader = command.ExecuteReader()) while (reader.Read())
                result.Add(new KeyValuePair<int, string>(Convert.ToInt32(reader["UserId"]), Text(reader, "FullName")));
            return result;
        }

        public void AssignDelivery(int adminId, long orderId, int agentId)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Delivery_Assign"))
            {
                command.Parameters.Add("@ActorUserId", SqlDbType.Int).Value = adminId;
                command.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = orderId;
                command.Parameters.Add("@AgentUserId", SqlDbType.Int).Value = agentId;
                command.ExecuteNonQuery();
            }
        }

        public int Register(string name, string email, string phone, string passwordHash)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Account_Register"))
            {
                command.Parameters.Add("@FullName", SqlDbType.NVarChar, 120).Value = name;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = phone;
                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = passwordHash;
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public AccountRecord FindAccount(string email)
        {
            using (var connection = OpenConnection())
            using (var command = Procedure(connection, "food.usp_Account_FindByEmail"))
            {
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email;
                using (var reader = command.ExecuteReader()) if (reader.Read()) return new AccountRecord
                {
                    UserId = Convert.ToInt32(reader["UserId"]), FullName = Text(reader, "FullName"),
                    Email = Text(reader, "Email"), PasswordHash = Text(reader, "PasswordHash"), Role = Text(reader, "Role")
                };
            }
            return null;
        }
    }

    public class AccountRecord
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }

    public class DeliveryAssignmentViewModel
    {
        public long AssignmentId { get; set; }
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string RestaurantName { get; set; }
        public string PickupAddress { get; set; }
        public string Recipient { get; set; }
        public string Phone { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class ReadyOrderViewModel
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string RestaurantName { get; set; }
        public string City { get; set; }
        public int? AgentUserId { get; set; }
    }
}
