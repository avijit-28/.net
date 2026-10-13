using System;

namespace MiniProject.Models
{
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsFeatured { get; set; }

        public bool InStock => Stock > 0;
        public decimal DiscountPercent => (OriginalPrice.HasValue && OriginalPrice > Price)
            ? Math.Round(((OriginalPrice.Value - Price) / OriginalPrice.Value) * 100)
            : 0;
    }
}
