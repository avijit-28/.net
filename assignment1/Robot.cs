using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    public abstract class Robot
    {
        public string name { get; set; }
        
        public Robot(string name)
        {
            this.name = name;
        }
        public void Start()
        {
            Console.WriteLine($"Robot {name} is starting.");
        }

        public abstract void PerformTask();
    }
    interface Isensor
    {
        void Detect();
    }
    interface Inevigable
    {
           void Navigate( string destination);
    }
    interface Ichargeable
    {
        void Charge();
    }

    public class Warehouse : Robot, Isensor, Inevigable, Ichargeable
    {
        public Warehouse(string name) : base(name)
        {
        }

        public override void PerformTask()
        {
            Console.WriteLine($"Robot {name} is performing warehouse tasks.");
        }
        public void Detect()
        {
            Console.WriteLine($"Robot {name} is detecting obstacles in the warehouse.");
        }
        public void Charge()
        {
            Console.WriteLine($"Robot {name} is charging.");
        }
        public void Navigate(string destination)
        {
            Console.WriteLine($"Robot {name} is navigating to {destination} in the warehouse.");
        }
    }

}
