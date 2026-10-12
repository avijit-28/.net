using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace ProductsManagement
{
    public class ProductRepo
    {
        private readonly string _conStr;

        public ProductRepo(string connectoinString)
        {
            _conStr = connectoinString;
        }

        public void AddProduct(Product product)
        {
            string query = "INSERT INTO Products (Id,Name, Price, Stock) VALUES (@Id,@Name, @Price, @Stock)";

            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Product added successfully.");
                }
            }
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            string query = "Select * from Products";

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Stock = reader.GetInt32(3)
                            });
                            //products.Add(product);

                        }
                    }
                }
                return products;
            }

        }


        public Product? GetProductById(int id)
        {
            string query = "Select * from Products where Id = @Id";
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Stock = reader.GetInt32(3)
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

        }

        public bool UpdateProduct(int id, string? name = null, decimal? price = null, int? stock = null)
        {
            var setClauses = new List<string>();

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@Id", id);

                // Agar name null NAHI hai, tabhi query me jodenge
                if (!string.IsNullOrWhiteSpace(name))
                {
                    setClauses.Add("[Name] = @Name");
                    cmd.Parameters.AddWithValue("@Name", name);
                }

                // Agar price null NAHI hai, tabhi query me jodenge
                if (price.HasValue)
                {
                    setClauses.Add("Price = @Price");
                    cmd.Parameters.AddWithValue("@Price", price.Value);
                }

                // Agar stock null NAHI hai, tabhi query me jodenge
                if (stock.HasValue)
                {
                    setClauses.Add("Stock = @Stock");
                    cmd.Parameters.AddWithValue("@Stock", stock.Value);
                }

                // Agar user ne kuch bhi nahi bheja
                if (setClauses.Count == 0)
                {
                    Console.WriteLine("Koi field provide nahi ki gayi.");
                    return false;
                }

                // Query sirf unhi columns ke liye banegi jo provide kiye gaye hain
                cmd.CommandText = $"UPDATE Products SET {string.Join(", ", setClauses)} WHERE Id = @Id";

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows>0 ? "Product updated Successfully" : "Product failed to update");
                return rows > 0;
            }
        }

        public bool Deleteproduct(int id)
        {
            string query = "Delete from Products where ID = @Id";
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? "Product deleted Successfully" : $"Product with Id {id} not found.");
                    return rows > 0;


                }

            }
        }
    
    }

}
