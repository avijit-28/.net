using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    abstract class SmartDevice
    {
        public abstract void TurnOn();
        public abstract void TurnOff();
        public abstract double GetEnergyConsumption();
    }

    class SmartLight : SmartDevice
    {
        public override void TurnOn()
        {
            Console.WriteLine("Smart Light is turned on.");
        }
        public override void TurnOff()
        {
            Console.WriteLine("Smart Light is turned off.");
        }
        public override double GetEnergyConsumption()
        {
            return 10.5; // Example energy consumption in watts
        }
    }

    class SmartThermostat : SmartDevice
    {
                public override void TurnOn()
        {
            Console.WriteLine("Smart Thermostat is turned on.");
        }
        public override void TurnOff()
        {
            Console.WriteLine("Smart Thermostat is turned off.");
        }
        public override double GetEnergyConsumption()
        {
            return 25; // Example energy consumption in watts
        }
    }

    class SmartAC : SmartDevice
    {
        public override void TurnOn()
        {
            Console.WriteLine("Smart AC is turned on.");
        }
        public override void TurnOff()
        {
            Console.WriteLine("Smart AC is turned off.");
        }
        public override double GetEnergyConsumption()
        {
            return 1500; // Example energy consumption in watts
        }
    }
}
