using RecordOptimization;
using System.Diagnostics;

List<Customers> customers = CreateCustomers(10000);
List<Orders> orders = CreateOrders(50000);
Console.WriteLine($"Customers : {customers.Count}");
Console.WriteLine($"Orders    : {orders.Count}");

Stopwatch stopwatch = Stopwatch.StartNew();

Dictionary<int, Customers> customerDictionary =
                customers.ToDictionary(c => c.CustomerId);

stopwatch.Stop();

Console.WriteLine(
    $"Dictionary creation time: {stopwatch.ElapsedMilliseconds} ms");

stopwatch.Restart();
List<OrderResult> results = new List<OrderResult>();

foreach (Orders order in orders)
{
    if (customerDictionary.TryGetValue(
        order.CustomerId,
        out Customers? customer))
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
stopwatch.Stop();



