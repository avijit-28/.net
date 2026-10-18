using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string PaymentMethod { get; set; }
        public bool HasReceipt { get; set; }
    }
}