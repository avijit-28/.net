using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class emp
    {
        public int id;
        public String name;
        public float salary;
        public emp(int i, String n, float s)
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
            Console.WriteLine("Destructor invoke");
        }

    }
}



