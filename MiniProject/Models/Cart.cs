using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniProject.Models
{
    [Serializable]
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public int TotalItemCount => Items.Sum(i => i.Quantity);

        public decimal Subtotal => Items.Sum(i => i.Subtotal);

        // Free shipping on orders over $100, otherwise $10 flat rate (or $0 if empty)
        public decimal Shipping => (Items.Count == 0 || Subtotal >= 100m) ? 0.00m : 10.00m;

        // Estimated Tax: 8%
        public decimal Tax => Math.Round(Subtotal * 0.08m, 2);

        public decimal GrandTotal => Subtotal + Shipping + Tax;

        public void AddItem(Product product, int quantity = 1)
        {
            if (product == null || quantity <= 0) return;

            var existing = Items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem(product, quantity));
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.Product.Id == productId);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
