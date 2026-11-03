using System;
using System.Collections.Generic;

namespace FoodArrival.Models
{
    public class PaymentGatewayViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string ItemsSummary { get; set; }
        public int TotalItemsCount { get; set; }
        public List<OrderDetail> Items { get; set; } = new List<OrderDetail>();
        public string PaymentMethod { get; set; } = "UPI";
        public string DefaultUpiId { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string CardExpiry { get; set; }
        public string CardCvv { get; set; }
        public string SelectedBank { get; set; }
    }
}
