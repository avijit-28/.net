using System;
using System.Collections.Generic;

namespace MiniProject.Models
{
    [Serializable]
    public class Order
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string PaymentMethod { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
