using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    public class OrderResult
    {
        public int OrderId { get; set; }
        //public int CustomerId { get;  set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public double Amount { get; set; }
    }
}
