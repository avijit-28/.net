using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using assignment1.IAllInterfaces;



namespace assignment1
{
    


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
        public void Charge( string batteryStatus)
        {
            Console.WriteLine($"Robot {Name} is {batteryStatus}.");
        }
    }
}
