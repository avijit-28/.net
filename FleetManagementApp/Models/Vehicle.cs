using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManagementApp.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VIN { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal OdometerReading { get; set; }
        public VehicleStatus Status { get; set; }
        public Vehicle()
        {
            Status = VehicleStatus.Active;
        }
        public Vehicle(string vin, string make, string model, int year, decimal odometerReading, VehicleStatus status = VehicleStatus.Active)
        {
            VIN = vin;
            Make = make;
            Model = model;
            Year = year;
            OdometerReading = odometerReading;
            Status = status;
        }
        public override string ToString()
        {
            return $"[ID: {Id}] {Year} {Make} {Model} | VIN: {VIN} | Mileage: {OdometerReading:N1} km | Status: {Status}";
        }
    }
}
