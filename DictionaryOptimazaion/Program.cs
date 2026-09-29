using DictionaryOptimazaion;

List<Customer> customers = new List<Customer>()
{
    new Customer ( 1, "Rahul" ),
    new Customer ( 2, "Priya" ),
    new Customer ( 3,  "Amit" ),
    new Customer ( 4,"Sneha" )
};

List<Order> orders = new List<Order>()
{
    new Order (  101,   3,  500),
    new Order ( 102,  1, 750 ),
    new Order (  103,  4,  1200 ),
    new Order (  104,  2,  900 ),
    new Order (  105,  10,  300 )
};

Dictionary<int, Customer> customerDictionary =
            customers.ToDictionary(c => c.Id);

foreach (var item in orders)
{
    if (customerDictionary.TryGetValue(item.CustomerId,out var customer))
    {
        Console.WriteLine(
                    $"Order ID: {item.OrderId}, " +
                    $"Customer: {customer.Name}, " +
                    $"Amount: ₹{item.Amount}");
    }

    else
    {
        Console.WriteLine(
            $"Order ID: {item.OrderId}, " +
            $"Customer not found");
    }

}