using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    internal class Vehicle
    {
        public string VehicleNumber { get; set; }   

        public void LogGPS()
        {
            Console.WriteLine($"Logging GPS for vehicle {VehicleNumber}");
        }

    }
    class MotorizedVehicle : Vehicle
    {
        protected double BatteryStatus { get; set; }    
        public void StartEngine()
        {
            Console.WriteLine($"Starting engine for vehicle {VehicleNumber}");
            Console.WriteLine("Engine started successfully.");
            Console.WriteLine($"Battery status: {BatteryStatus}");
        }

    }

    class ElectricVehicle : MotorizedVehicle
    {
        public ElectricVehicle(double BatteryStatus) {
            this.BatteryStatus = BatteryStatus;
        }

        public void calculateRange()
        {
            double range = BatteryStatus * 5; // Example calculation
            Console.WriteLine($"Estimated range for vehicle {VehicleNumber}: {range} km");
        }
    }
}
