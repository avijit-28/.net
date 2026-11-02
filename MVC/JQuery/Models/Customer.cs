using System;

namespace JQuery.Models
{
    /// <summary>
    /// Customer model representing the database entity structure.
    /// Maps to CustomerID, ContactName, City, Country as utilized in the jQuery DataTable and JSGrid.
    /// </summary>
    public class Customer
    {
        public int CustomerID { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
