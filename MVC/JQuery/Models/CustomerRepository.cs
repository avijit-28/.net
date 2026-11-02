using System.Collections.Generic;

namespace JQuery.Models
{
    /// <summary>
    /// In-memory repository providing customer records for demonstration.
    /// Provides consistent data for Entity Framework / LINQ queries in HomeController.
    /// </summary>
    public static class CustomerRepository
    {
        private static readonly List<Customer> _customers = new List<Customer>
        {
            new Customer { CustomerID = 1001, ContactName = "Maria Anders", City = "Berlin", Country = "Germany", Email = "maria.anders@example.de", Phone = "+49 30 0074321" },
            new Customer { CustomerID = 1002, ContactName = "Ana Trujillo", City = "Mexico D.F.", Country = "Mexico", Email = "ana.trujillo@example.mx", Phone = "+52 5 555 4729" },
            new Customer { CustomerID = 1003, ContactName = "Antonio Moreno", City = "Mexico D.F.", Country = "Mexico", Email = "antonio.moreno@example.mx", Phone = "+52 5 555 3932" },
            new Customer { CustomerID = 1004, ContactName = "Thomas Hardy", City = "London", Country = "UK", Email = "thomas.hardy@example.co.uk", Phone = "+44 171 555 7788" },
            new Customer { CustomerID = 1005, ContactName = "Christina Berglund", City = "Lulea", Country = "Sweden", Email = "berglunds@example.se", Phone = "+46 920 12345" },
            new Customer { CustomerID = 1006, ContactName = "Hanna Moos", City = "Mannheim", Country = "Germany", Email = "hanna.moos@example.de", Phone = "+49 621 08525" },
            new Customer { CustomerID = 1007, ContactName = "Frederique Citeaux", City = "Strasbourg", Country = "France", Email = "frederique@example.fr", Phone = "+33 88 60 15 31" },
            new Customer { CustomerID = 1008, ContactName = "Martin Sommer", City = "Madrid", Country = "Spain", Email = "martin.sommer@example.es", Phone = "+34 91 555 22 82" },
            new Customer { CustomerID = 1009, ContactName = "Laurence Lebihan", City = "Marseille", Country = "France", Email = "laurence@example.fr", Phone = "+33 91 24 45 40" },
            new Customer { CustomerID = 1010, ContactName = "Elizabeth Lincoln", City = "Tsawwassen", Country = "Canada", Email = "elizabeth.l@example.ca", Phone = "+1 604 555 4729" },
            new Customer { CustomerID = 1011, ContactName = "Victoria Ashworth", City = "London", Country = "UK", Email = "v.ashworth@example.co.uk", Phone = "+44 171 555 2555" },
            new Customer { CustomerID = 1012, ContactName = "Patricio Simpson", City = "Buenos Aires", Country = "Argentina", Email = "patricio@example.ar", Phone = "+54 1 335 5555" },
            new Customer { CustomerID = 1013, ContactName = "Francisco Chang", City = "Mexico D.F.", Country = "Mexico", Email = "francisco@example.mx", Phone = "+52 5 555 7293" },
            new Customer { CustomerID = 1014, ContactName = "Yang Wang", City = "Bern", Country = "Switzerland", Email = "yang.wang@example.ch", Phone = "+41 31 381 2345" },
            new Customer { CustomerID = 1015, ContactName = "Pedro Afonso", City = "Sao Paulo", Country = "Brazil", Email = "pedro.afonso@example.br", Phone = "+55 11 555 7640" },
            new Customer { CustomerID = 1016, ContactName = "Elizabeth Brown", City = "London", Country = "UK", Email = "elizabeth.b@example.co.uk", Phone = "+44 171 555 2282" },
            new Customer { CustomerID = 1017, ContactName = "Sven Ottlieb", City = "Aachen", Country = "Germany", Email = "sven.ottlieb@example.de", Phone = "+49 241 039123" },
            new Customer { CustomerID = 1018, ContactName = "Janine Labrune", City = "Nantes", Country = "France", Email = "janine.l@example.fr", Phone = "+33 40 67 88 88" },
            new Customer { CustomerID = 1019, ContactName = "Ann Devon", City = "London", Country = "UK", Email = "ann.devon@example.co.uk", Phone = "+44 171 555 0297" },
            new Customer { CustomerID = 1020, ContactName = "Roland Mendel", City = "Graz", Country = "Austria", Email = "roland.mendel@example.at", Phone = "+43 7675 3425" },
            new Customer { CustomerID = 1021, ContactName = "Aria Cruz", City = "Sao Paulo", Country = "Brazil", Email = "aria.cruz@example.br", Phone = "+55 11 555 9857" },
            new Customer { CustomerID = 1022, ContactName = "Diego Roel", City = "Madrid", Country = "Spain", Email = "diego.roel@example.es", Phone = "+34 91 555 94 44" },
            new Customer { CustomerID = 1023, ContactName = "Martine Rancé", City = "Lille", Country = "France", Email = "martine.rance@example.fr", Phone = "+33 20 16 10 16" },
            new Customer { CustomerID = 1024, ContactName = "Maria Larsson", City = "Bräcke", Country = "Sweden", Email = "maria.larsson@example.se", Phone = "+46 693 12345" },
            new Customer { CustomerID = 1025, ContactName = "Peter Franken", City = "München", Country = "Germany", Email = "peter.franken@example.de", Phone = "+49 89 087755" }
        };

        public static List<Customer> GetCustomers()
        {
            return new List<Customer>(_customers);
        }
    }
}
