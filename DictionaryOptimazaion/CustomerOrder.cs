using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryOptimazaion
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Customer(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }

    public class Order
    {
        public int OrderId { get; set;  }
        public int CustomerId { get; set; }

        public double Amount { get; set; }

        public Order (int OrderId, int CustomerId, double Amount)
        {
            this.OrderId = OrderId;
            this.CustomerId = CustomerId;
            this.Amount = Amount;

        }
    }


}
