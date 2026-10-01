using System;
using System.Collections.Generic;
using System.Text;

namespace Vehicle
{
    public class Car
    {
        //private readonly IEngine _engine;
        //public IEngine Engine { get; private set; }

        // Dependency bahar se inject ho rahi hai
        //public Car(IEngine engine)
        //{
        //    _engine = engine;
        //}
        //public void SetEngine(IEngine engine)
        //{
        //    Engine = engine;
        //}

        //public IEngine getEngine() {
        //    return Engine;
        //}
        //public void Drive()
        //{
        //    _engine.Start();
        //    Console.WriteLine("Car is driving.");
        //}

        public void StartEngine(IEngine engine) {
            engine.Start();

        }
    }

}
