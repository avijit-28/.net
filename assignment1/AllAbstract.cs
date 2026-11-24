using System;
namespace assignment1
{

    // BaseRobot abstract class
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

}

   