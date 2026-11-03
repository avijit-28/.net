using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string path = @"D:\avi\C#\MVC\FoodArrival\Web.config";
        string content = File.ReadAllText(path);
        content = Regex.Replace(content, @"<authentication\s+mode=""Forms"">[\s\S]*?</authentication>", @"<authentication mode=""None"" />");
        File.WriteAllText(path, content);
        Console.WriteLine("Updated Web.config successfully.");
    }
}
