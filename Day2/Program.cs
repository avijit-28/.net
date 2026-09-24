// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
Console.WriteLine("Enter a new Key:");
int var1 = Console.Read();
Console.WriteLine($"ASCII Value of the Entered Key is: {var1}");

Console.WriteLine("Enter Another Key:");
ConsoleKeyInfo var2 = Console.ReadKey();
Console.WriteLine($"\nASCII Value of the Entered Key is: {(int)var2.KeyChar}");
//Console.WriteLine($"\nEntered Key: {var2.Key} KeyChar:{var2.KeyChar} ASCII:{(int)var2.KeyChar}");
//Console.WriteLine("Press Any Key to Terminate");
//Console.ReadKey();

