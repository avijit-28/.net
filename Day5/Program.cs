using Day5;

bool IsEqual = GenericsDemo.AreEqual<int>(10, 10);

if (IsEqual)
{
    Console.WriteLine("BOTH ARE EQUAL");
}
else
{
    Console.WriteLine("Both are not Equal");
}
Console.ReadKey();