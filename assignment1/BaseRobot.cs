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
        public string Name { get; set; }
        public BaseRobot(string name)
        {
            Name = name;
        }
        public void Start()
        {
            Console.WriteLine($"Robot {Name} is starting.");
        }
        // Must be implemented by specific robot types
        public abstract void PerformTask();
    }


    public class WarehouseRobot : BaseRobot, ISensor, INavigable, IChargeable
    {
        public WarehouseRobot(string name) : base(name)
        {
        }
        // Overriding abstract method
        public override void PerformTask()
        {
            Console.WriteLine($"Robot {Name} is performing warehouse inventory handling.");
        }
        // Implementing ISensor
        public void Detect()
        {
            Console.WriteLine($"Robot {Name} is detecting obstacles using LiDAR sensors.");
        }
        // Implementing INavigable
        public void Navigate(string destination)
        {
            Console.WriteLine($"Robot {Name} is navigating to {destination}.");
        }
        // Implementing IChargeable
        public void Charge()
        {
            Console.WriteLine($"Robot {Name} is docking and charging battery.");
        }
    }
}
