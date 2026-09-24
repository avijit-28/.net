using System;

namespace ConsoleApp1
{
    public class emp
    {
        public int id;
        public string name;
        public float salary;

        public emp(int i, string n, float s)
        {
            id = i;
            name = n;
            salary = s;
        }

        public void display()
        {
            Console.WriteLine(id + " " + name + " " + salary);
        }

        ~emp()
        {
            Console.WriteLine("Destructor invoked");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            emp e1 = new emp(101, "Sonoo", 890000f);
            emp e2 = new emp(102, "Mahesh", 490000f);

            e1 = null;
            e2 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Console.WriteLine("Press Enter");
            Console.ReadLine();
        }
    }
}