using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    internal class Vehicle
    {
        public string VehicleId { get; set; }
        public string VehicleNumber { get; set; }   

        protected (double Latitude, double Longitude) GPSCoordinates { get; set; }

        private readonly string _secretCode;

        public Vehicle(string vehicleId, string vehicleNumber, double latitude, double longitude, string secretCode)
        {
            VehicleId = vehicleId;
            VehicleNumber = vehicleNumber;
            GPSCoordinates = (latitude, longitude);
            _secretCode = secretCode;
        }

        public void LogGPS()
        {
            Console.WriteLine($"Logging GPS for vehicle {VehicleId}");
        }
        public virtual void StartEngine() 
        {
            Console.WriteLine("[Level 1 - Vehicle] Generic vehicle engine start.");
        }

    }

    class MotorizedVehicle : Vehicle
    {
        public int Horsepower { get; set; }
        protected double FuelOrBatteryLevel { get; set; }   
        
        public MotorizedVehicle(string vehicleId, string vehicleNumber, double latitude, double longitude, string secretCode, int horsepower, double fuelOrBatteryLevel)
            : base(vehicleId, vehicleNumber, latitude, longitude, secretCode)
        {
            Horsepower = horsepower;
            FuelOrBatteryLevel = fuelOrBatteryLevel;
        }
        public override void StartEngine()
        {
            Console.WriteLine($"[Level 2 - MotorizedVehicle] Starting engine for vehicle {VehicleNumber} ({Horsepower} HP)...");
            Console.WriteLine($"Engine started successfully. Battery/Fuel Level: {FuelOrBatteryLevel}%");
        }
        public virtual void UpdateLocation(double newLatitude, double newLongitude)
        {
            GPSCoordinates = (newLatitude, newLongitude);
            Console.WriteLine($"[Level 2 - MotorizedVehicle] New coordinates updated to: ({GPSCoordinates.Latitude}, {GPSCoordinates.Longitude})");
        }

    }

    class ElectricVehicle : MotorizedVehicle
    {
       

        public ElectricVehicle(string vehicleId, string vehicleNumber, double latitude, double longitude, string secretCode, int horsepower, double batteryLevel)
            : base(vehicleId, vehicleNumber, latitude, longitude, secretCode, horsepower, batteryLevel)
        {
        }
        public override void StartEngine()
        {
            Console.WriteLine($"[Level 3 - ElectricVehicle] Electric motor started silently for {VehicleNumber}. Battery: {FuelOrBatteryLevel}%");
        }

        public void calculateRange()
        {
            double range = FuelOrBatteryLevel * 5; // Example calculation
            Console.WriteLine($"[Level 3 - ElectricVehicle] Estimated range for {VehicleNumber}: {range} km");
        }

        public override void UpdateLocation(double newLatitude, double newLongitude)
        {
            GPSCoordinates = (newLatitude, newLongitude);
            Console.WriteLine($"[Level 3 - ElectricVehicle] New coordinates updated to: ({GPSCoordinates.Latitude}, {GPSCoordinates.Longitude})");
        }
    }
}
