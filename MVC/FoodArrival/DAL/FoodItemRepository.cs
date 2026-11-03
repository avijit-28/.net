using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class FoodItemRepository
    {
        private FoodItem MapRow(DataRow row)
        {
            var item = new FoodItem
            {
                FoodItemId = Convert.ToInt32(row["FoodItemId"]),
                ItemName = row["ItemName"].ToString(),
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                Price = Convert.ToDecimal(row["Price"]),
                ImageUrl = row["ImageUrl"] != DBNull.Value ? row["ImageUrl"].ToString() : null,
                IsAvailable = Convert.ToBoolean(row["IsAvailable"])
            };

            if (row.Table.Columns.Contains("Category") && row["Category"] != DBNull.Value)
            {
                item.Category = row["Category"].ToString();
            }

            if (row.Table.Columns.Contains("RestaurantId") && row["RestaurantId"] != DBNull.Value)
            {
                item.RestaurantId = Convert.ToInt32(row["RestaurantId"]);
            }

            if (row.Table.Columns.Contains("RestaurantName") && row["RestaurantName"] != DBNull.Value)
            {
                item.RestaurantName = row["RestaurantName"].ToString();
            }

            if (row.Table.Columns.Contains("CreatedAt") && row["CreatedAt"] != DBNull.Value)
            {
                item.CreatedAt = Convert.ToDateTime(row["CreatedAt"]);
            }

            if (row.Table.Columns.Contains("UpdatedAt") && row["UpdatedAt"] != DBNull.Value)
            {
                item.UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]);
            }

            return item;
        }

        public List<FoodItem> GetAll()
        {
            var list = new List<FoodItem>();
            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_GetAll");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRow(row));
            }
            return list;
        }

        public List<FoodItem> GetAvailable()
        {
            var list = new List<FoodItem>();
            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_GetAvailable");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRow(row));
            }
            return list;
        }

        public FoodItem GetById(int foodItemId)
        {
            var parameters = new[]
            {
                new SqlParameter("@FoodItemId", foodItemId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_GetById", parameters);
            if (dt.Rows.Count > 0)
            {
                return MapRow(dt.Rows[0]);
            }
            return null;
        }

        public List<FoodItem> Search(string keyword)
        {
            var list = new List<FoodItem>();
            var parameters = new[]
            {
                new SqlParameter("@Search", keyword ?? string.Empty)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_Search", parameters);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRow(row));
            }
            return list;
        }

        public List<FoodItem> GetByCategory(string category)
        {
            var all = GetAvailable();
            if (string.IsNullOrWhiteSpace(category) || category.Equals("All items", StringComparison.OrdinalIgnoreCase))
            {
                return all;
            }
            return all.Where(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public bool Insert(FoodItem item, out string message, out int foodItemId)
        {
            message = string.Empty;
            foodItemId = 0;

            var parameters = new[]
            {
                new SqlParameter("@ItemName", item.ItemName ?? string.Empty),
                new SqlParameter("@Description", (object)item.Description ?? DBNull.Value),
                new SqlParameter("@Price", item.Price),
                new SqlParameter("@ImageUrl", (object)item.ImageUrl ?? DBNull.Value),
                new SqlParameter("@IsAvailable", item.IsAvailable),
                new SqlParameter("@Category", (object)item.Category ?? "Snacks")
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_Insert", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                if (result == 1)
                {
                    if (dt.Rows[0]["FoodItemId"] != DBNull.Value)
                    {
                        foodItemId = Convert.ToInt32(dt.Rows[0]["FoodItemId"]);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool Update(FoodItem item, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@FoodItemId", item.FoodItemId),
                new SqlParameter("@ItemName", item.ItemName ?? string.Empty),
                new SqlParameter("@Description", (object)item.Description ?? DBNull.Value),
                new SqlParameter("@Price", item.Price),
                new SqlParameter("@ImageUrl", (object)item.ImageUrl ?? DBNull.Value),
                new SqlParameter("@IsAvailable", item.IsAvailable),
                new SqlParameter("@Category", (object)item.Category ?? "Snacks")
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_Update", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public bool Delete(int foodItemId, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@FoodItemId", foodItemId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_FoodItem_Delete", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public List<FoodItem> GetByRestaurant(int restaurantId)
        {
            var list = new List<FoodItem>();
            string sql = @"
                SELECT FoodItemId, ItemName, Description, Price, ImageUrl, IsAvailable, Category, IsVerified, CreatedAt
                FROM dbo.FoodItems
                WHERE (RestaurantId = @RestaurantId OR RestaurantId IS NULL)
                ORDER BY FoodItemId DESC";
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new FoodItem
                            {
                                FoodItemId = Convert.ToInt32(reader["FoodItemId"]),
                                ItemName = reader["ItemName"].ToString(),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : null,
                                IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "Snacks",
                                IsVerified = Convert.ToBoolean(reader["IsVerified"])
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }
    }
}
