using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniProject.Models
{
    public static class ProductRepository
    {
        private static readonly List<Product> _products;

        static ProductRepository()
        {
            _products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Quantum Wireless Noise-Cancelling Headphones",
                    Category = "Audio",
                    Price = 199.99m,
                    OriginalPrice = 249.99m,
                    Description = "Experience studio-grade acoustics with active noise cancellation, 40-hour battery life, and ultra-plush memory foam earcups.",
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&auto=format&fit=crop&q=80",
                    Stock = 25,
                    Rating = 4.8,
                    ReviewCount = 142,
                    IsFeatured = true
                },
                new Product
                {
                    Id = 2,
                    Name = "AeroBook Pro 15 Ultra Laptop",
                    Category = "Laptops",
                    Price = 1199.00m,
                    OriginalPrice = 1349.00m,
                    Description = "Powered by latest 10-core processor, 16GB unified RAM, 512GB NVMe SSD, and vivid 15.6-inch Retina-grade display.",
                    ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600&auto=format&fit=crop&q=80",
                    Stock = 12,
                    Rating = 4.9,
                    ReviewCount = 89,
                    IsFeatured = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Chronos Smart Watch S5",
                    Category = "Wearables",
                    Price = 159.50m,
                    OriginalPrice = 189.99m,
                    Description = "Advanced health metrics tracking including SpO2, continuous heart rate, sleep stages, built-in GPS, and 50m water resistance.",
                    ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&auto=format&fit=crop&q=80",
                    Stock = 30,
                    Rating = 4.6,
                    ReviewCount = 210,
                    IsFeatured = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Apex Pro Optical Gaming Mouse",
                    Category = "Gaming",
                    Price = 59.99m,
                    OriginalPrice = 79.99m,
                    Description = "26,000 DPI optical sensor, ultra-lightweight 63g honeycomb chassis, PTFE glide feet, and customizable RGB lighting.",
                    ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=600&auto=format&fit=crop&q=80",
                    Stock = 45,
                    Rating = 4.7,
                    ReviewCount = 98,
                    IsFeatured = false
                },
                new Product
                {
                    Id = 5,
                    Name = "VibeSound 360 Portable Bluetooth Speaker",
                    Category = "Audio",
                    Price = 79.00m,
                    OriginalPrice = 99.00m,
                    Description = "Rugged IP67 waterproof speaker with dual bass radiators, party link pairing, and 18-hour continuous playtime.",
                    ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=600&auto=format&fit=crop&q=80",
                    Stock = 18,
                    Rating = 4.5,
                    ReviewCount = 76,
                    IsFeatured = false
                },
                new Product
                {
                    Id = 6,
                    Name = "Nova 4K Mirrorless Cinema Camera",
                    Category = "Cameras",
                    Price = 899.99m,
                    OriginalPrice = 999.99m,
                    Description = "Capture stunning cinematic video in 4K at 60fps with in-body 5-axis stabilization, fast eye-autofocus, and interchangeable mount.",
                    ImageUrl = "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=600&auto=format&fit=crop&q=80",
                    Stock = 8,
                    Rating = 4.9,
                    ReviewCount = 54,
                    IsFeatured = true
                },
                new Product
                {
                    Id = 7,
                    Name = "Kinesis Mechanical Ergonomic Keyboard",
                    Category = "Accessories",
                    Price = 129.99m,
                    OriginalPrice = 149.99m,
                    Description = "Hot-swappable tactile brown switches, PBT double-shot keycaps, aluminum top plate, and Bluetooth 5.2 multi-device switching.",
                    ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=600&auto=format&fit=crop&q=80",
                    Stock = 22,
                    Rating = 4.7,
                    ReviewCount = 115,
                    IsFeatured = false
                },
                new Product
                {
                    Id = 8,
                    Name = "Zenith Horizon Curved 34-inch Monitor",
                    Category = "Laptops",
                    Price = 449.00m,
                    OriginalPrice = 499.00m,
                    Description = "UWQHD (3440x1440) 144Hz 1ms curved gaming monitor with HDR400, USB-C 90W power delivery, and PIP/PBP productivity modes.",
                    ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=600&auto=format&fit=crop&q=80",
                    Stock = 10,
                    Rating = 4.8,
                    ReviewCount = 67,
                    IsFeatured = true
                },
                new Product
                {
                    Id = 9,
                    Name = "PowerPulse 65W GaN Fast Charger",
                    Category = "Accessories",
                    Price = 34.99m,
                    OriginalPrice = 44.99m,
                    Description = "Compact GaN III charging station with dual USB-C ports and USB-A port. Fast charges laptops, phones, and tablets simultaneously.",
                    ImageUrl = "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?w=600&auto=format&fit=crop&q=80",
                    Stock = 60,
                    Rating = 4.6,
                    ReviewCount = 188,
                    IsFeatured = false
                },
                new Product
                {
                    Id = 10,
                    Name = "Nebula VR Immersive Headset",
                    Category = "Gaming",
                    Price = 399.00m,
                    OriginalPrice = 450.00m,
                    Description = "Standalone virtual reality system with pancake lenses, 120Hz refresh rate, spatial audio, and ergonomic haptic touch controllers.",
                    ImageUrl = "https://images.unsplash.com/photo-1622979135225-d2ba269bc1df?w=600&auto=format&fit=crop&q=80",
                    Stock = 14,
                    Rating = 4.7,
                    ReviewCount = 83,
                    IsFeatured = false
                },
                new Product
                {
                    Id = 11,
                    Name = "PulseFit Smart Fitness Band",
                    Category = "Wearables",
                    Price = 49.99m,
                    OriginalPrice = 69.99m,
                    Description = "Lightweight fitness tracker with AMOLED touch display, 14-day battery life, 30 sports modes, and sleep/stress monitoring.",
                    ImageUrl = "https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?w=600&auto=format&fit=crop&q=80",
                    Stock = 35,
                    Rating = 4.4,
                    ReviewCount = 129,
                    IsFeatured = false
                },
                new Product
                {
                    Id = 12,
                    Name = "StudioPro Cardioid USB Microphone",
                    Category = "Audio",
                    Price = 89.99m,
                    OriginalPrice = 119.99m,
                    Description = "High-definition 192kHz/24bit condensation microphone with built-in pop filter, zero-latency headphone monitoring, and mute button.",
                    ImageUrl = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=600&auto=format&fit=crop&q=80",
                    Stock = 20,
                    Rating = 4.8,
                    ReviewCount = 92,
                    IsFeatured = false
                }
            };
        }

        public static List<Product> GetAllProducts()
        {
            return _products.ToList();
        }

        public static Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public static List<string> GetCategories()
        {
            return _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        }

        public static List<Product> GetFeaturedProducts()
        {
            return _products.Where(p => p.IsFeatured).ToList();
        }

        public static List<Product> GetRelatedProducts(int currentProductId, string category, int count = 3)
        {
            return _products
                .Where(p => p.Id != currentProductId && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Take(count)
                .ToList();
        }

        public static List<Product> FilterProducts(string query, string category, string sortBy)
        {
            var list = _products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLowerInvariant();
                list = list.Where(p => p.Name.ToLowerInvariant().Contains(q) ||
                                       p.Description.ToLowerInvariant().Contains(q) ||
                                       p.Category.ToLowerInvariant().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                list = list.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            switch (sortBy)
            {
                case "price_asc":
                    list = list.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    list = list.OrderByDescending(p => p.Price);
                    break;
                case "rating":
                    list = list.OrderByDescending(p => p.Rating);
                    break;
                case "name":
                    list = list.OrderBy(p => p.Name);
                    break;
                default:
                    // default / featured
                    list = list.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.Id);
                    break;
            }

            return list.ToList();
        }
    }
}
