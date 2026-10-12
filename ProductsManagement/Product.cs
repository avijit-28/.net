using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsManagement
{
    public class Product {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
