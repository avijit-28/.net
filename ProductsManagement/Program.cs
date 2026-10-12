using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ProductsManagement
{
    public class Program
    {
        static void Main(string[] args)
        {
            // 1. appsettings.json se connection string read karna
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Note: Aapke JSON me "DefaultConnecttion" (double 't') likha tha, isliye dono check kar rahe hain
            string? connectionString = configuration.GetConnectionString("sqlDefaultConnection")
                                    //?? configuration.GetConnectionString("DefaultConnection")
                                    //?? "Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234;Encrypt=True;Trust Server Certificate=True;";

            ProductRepo repo = new ProductRepo(connectionString);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("        PRODUCT MANAGEMENT SYSTEM         ");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. View All Products");
                Console.WriteLine("2. View Product By ID");
                Console.WriteLine("3. Add New Product");
                Console.WriteLine("4. Update Product (Press Enter to skip any field)");
                Console.WriteLine("5. Delete Product");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice (1-6): ");

                
                string? choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAllProducts(repo);
                        break;

                    case "2":
                        ShowProductById(repo);
                        break;

                    case "3":
                        AddNewProduct(repo);
                        break;

                    case "4":
                        UpdateProductDetails(repo);
                        break;

                    case "5":
                        DeleteProductById(repo);
                        break;

                    case "6":
                        Console.WriteLine("Exiting application. Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please choose between 1 and 6.");
                        break;
                }
                Console.WriteLine("Press Any Key...");
                Console.ReadKey();
            }
            
        }
        

        // 1. View All Products
        static void ShowAllProducts(ProductRepo repo)
        {
            List<Product> products = repo.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("No products found in the database.");
                return;
            }

            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-6} | {"Name",-20} | {"Price",-12} | {"Stock",-8}");
            Console.WriteLine("------------------------------------------------------------------");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id,-6} | {p.Name,-20} | {p.Price,-12:F2} | {p.Stock,-8}");
            }
            Console.WriteLine("------------------------------------------------------------------");
        }

        // 2. View Product By ID
        static void ShowProductById(ProductRepo repo)
        {
            Console.Write("Enter Product ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Product? product = repo.GetProductById(id);
            if (product != null)
            {
                Console.WriteLine($"\nProduct Found:");
                Console.WriteLine($"ID:    {product.Id}");
                Console.WriteLine($"Name:  {product.Name}");
                Console.WriteLine($"Price: ${product.Price:F2}");
                Console.WriteLine($"Stock: {product.Stock}");
            }
            else
            {
                Console.WriteLine($"Product with ID {id} not found.");
            }
        }

        // 3. Add Product
        static void AddNewProduct(ProductRepo repo)
        {
            Console.Write("Enter Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid Price.");
                return;
            }

            Console.Write("Enter Stock: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Invalid Stock.");
                return;
            }

            Product newProduct = new Product
            {
                Id = id,
                Name = name,
                Price = price,
                Stock = stock
            };

            repo.AddProduct(newProduct);
        }

        // 4. Update Product (Partial/Variable Update)
        static void UpdateProductDetails(ProductRepo repo)
        {
            Console.Write("Enter Product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var existing = repo.GetProductById(id);
            if (existing == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                return;
            }

            Console.WriteLine($"\nCurrent Details -> Name: '{existing.Name}', Price: {existing.Price:F2}, Stock: {existing.Stock}");
            Console.WriteLine("Note: Press [ENTER] to keep existing value.\n");

            // Name
            Console.Write($"New Name (Leave blank to keep '{existing.Name}'): ");
            string? inputName = Console.ReadLine()?.Trim();
            string? nameToUpdate = string.IsNullOrEmpty(inputName) ? null : inputName;

            // Price
            Console.Write($"New Price (Leave blank to keep '{existing.Price:F2}'): ");
            string? inputPrice = Console.ReadLine()?.Trim();
            decimal? priceToUpdate = decimal.TryParse(inputPrice, out decimal p) ? p : null;

            // Stock
            Console.Write($"New Stock (Leave blank to keep '{existing.Stock}'): ");
            string? inputStock = Console.ReadLine()?.Trim();
            int? stockToUpdate = int.TryParse(inputStock, out int s) ? s : null;

            repo.UpdateProduct(id, nameToUpdate, priceToUpdate, stockToUpdate);
        }

        // 5. Delete Product
        static void DeleteProductById(ProductRepo repo)
        {
            Console.Write("Enter Product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            repo.Deleteproduct(id);
        }

    
    }
}