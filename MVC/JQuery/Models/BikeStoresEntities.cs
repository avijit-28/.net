using System.Collections.Generic;

namespace JQuery.Models
{
    /// <summary>
    /// Represents the Entity Framework DbContext container as specified in the workflow.
    /// Provides access to the Customers dataset for LINQ queries.
    /// </summary>
    public class BikeStoresEntities
    {
        public List<Customer> Customers { get; set; }

        public BikeStoresEntities()
        {
            Customers = CustomerRepository.GetCustomers();
        }
    }
}
