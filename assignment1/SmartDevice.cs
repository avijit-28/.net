using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    
    class SmartLight : SmartDevice
    {
        public SmartLight(string deviceName) : base(deviceName) { }
        public override void TurnOn()
        {
            Console.WriteLine($"Smart Light ({DeviceName}) is turned on.");
        }
        public override void TurnOff()
        {
            Console.WriteLine($"Smart Light ({DeviceName}) is turned off.");
        }
        public override double GetEnergyConsumption()
        {
            return 10.5; 
        }
    }

    class SmartThermostat : SmartDevice
    {
        public SmartThermostat(string deviceName) : base(deviceName) { }

        public override void TurnOn()
        {
            Console.WriteLine($"Smart Thermostat ({DeviceName}) is turned on.");
        }
        public override void TurnOff()
        {
            Console.WriteLine($"Smart Thermostat ({DeviceName}) is turned off.");
        }
        public override double GetEnergyConsumption()
        {
            return 25; // Example energy consumption in watts
        }
    }

    class SmartAC : SmartDevice
    {
        public SmartAC(string deviceName) : base(deviceName) { }
        public override void TurnOn()
        {
            Console.WriteLine($"Smart AC ({DeviceName}) is turned on.");
        }
        public override void TurnOff()
        {
            Console.WriteLine($"Smart AC ({DeviceName}) is turned off.");
        }
        public override double GetEnergyConsumption()
        {
            return 1500; 
        }
    }
}
