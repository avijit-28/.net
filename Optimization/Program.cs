using ConsoleApp2;
using System.Diagnostics;

List<Customers> customers = CreateCustomers(10000);
List<Orders> orders = CreateOrders(50000);

Console.WriteLine($"Customers : {customers.Count}");
Console.WriteLine($"Orders    : {orders.Count}");

//Stopwatch stopwatch = Stopwatch.StartNew();

//Dictionary<int, Customers> customerDictionary =
//    customers.ToDictionary(c => c.CustomerId);

//stopwatch.Stop();

Console.WriteLine();
//Console.WriteLine(
    //$"Dictionary creation time: {stopwatch.ElapsedMilliseconds} ms");

//stopwatch.Restart();
Stopwatch stopwatch = Stopwatch.StartNew();

List<OrderResult> results = new List<OrderResult>();

// ----------------------------------------------------
//  Find customer using Dictionary
// ----------------------------------------------------

//foreach (Orders order in orders)
//{
//    if (customerDictionary.TryGetValue(
//            order.CustomerId,
//            out Customers? customer))
//    {
//        results.Add(new OrderResult
//        {
//            OrderId = order.OrderId,
//            CustomerName = customer.CustomerName,
//            CustomerEmail = customer.CustomerEmail,
//            Amount = order.Amount
//        });
//    }
//}

// ----------------------------------------------------
//  Find customer using List
// ----------------------------------------------------

foreach (Orders order in orders)
{
    Customers? customer = customers
        .FirstOrDefault(c => c.CustomerId == order.CustomerId);

    if (customer != null)
    {
        results.Add(new OrderResult
        {
            OrderId = order.OrderId,
            CustomerName = customer.CustomerName,
            CustomerEmail = customer.CustomerEmail,
            Amount = order.Amount
        });
    }
}
//----------------------------------------

stopwatch.Stop();


Console.WriteLine(
    $"Order processing time: {stopwatch.ElapsedMilliseconds} ms");

Console.WriteLine(
    $"Matched orders: {results.Count}");

Console.WriteLine();
Console.WriteLine("First 10 results:");
Console.WriteLine("----------------------------");

foreach (var result in results.Take(10))
{
    Console.WriteLine(
        $"Order: {result.OrderId}, " +
        $"Customer: {result.CustomerName}, " +
        $"Amount: {result.Amount:C}");
}

Console.ReadLine();

static List<Customers> CreateCustomers(int count)
{
    List<Customers> customers = new List<Customers>();

    for (int i = 1; i <= count; i++)
    {
        customers.Add(new Customers
        {
            CustomerId = i,
            CustomerName = $"Customer {i}",
            CustomerEmail = $"customer{i}@example.com"
        });
    }

    return customers;
}

static List<Orders> CreateOrders(int count)
{
    List<Orders> orders = new List<Orders>();

    Random random = new Random();

    for (int i = 1; i <= count; i++)
    {
        orders.Add(new Orders
        {
            OrderId = i,

            // Customer IDs between 1 and 10000
            CustomerId = random.Next(1, 10001),

            Amount = random.Next(100, 10000)
        });
    }

    return orders;
}


