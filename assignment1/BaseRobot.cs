using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using assignment1.IAllInterfaces;


namespace assignment1
{
    public abstract class BaseRobot
    {
        public string name { get; set; }

        public BaseRobot(string name)
        {
            this.name = name;
        }
        public void Start()
        {
            Console.WriteLine($"Robot {name} is starting.");
        }

        public abstract void PerformTask();
    }


    public class WarehouseRobot : BaseRobot, Isensor, Inevigable, Ichargeable
    {
        public WarehouseRobot(string name) : base(name)
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
