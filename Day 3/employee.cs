using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_3
{
    public class employee
    {
        public int id;
        public string name;
        public float salary;
        public employee(int id, string name, float salary)
        {
            this.id = id;
            this.name = name;
            this.salary = salary;
        }
        public void display()
        {
            Console.WriteLine(id+" "+name+" "+salary);
        }
    }
    
}

