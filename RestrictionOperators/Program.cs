using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestrictionOperators
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            // Func<int, bool> predicate = i => i % 2 == 0;
            // IEnumerable<int> evenNumbers = numbers.Where(predicate);
            IEnumerable<int> evenNumbers = numbers.Where(num => num % 2 == 0);
            foreach (int evenNumber in evenNumbers)
            {
                Console.WriteLine(evenNumber);
            }
        }
    }
}
