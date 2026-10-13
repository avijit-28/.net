using System;

namespace MiniProject.Models
{
    [Serializable]
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal Subtotal => Product != null ? Product.Price * Quantity : 0m;

        public CartItem()
        {
        }

        public CartItem(Product product, int quantity = 1)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
