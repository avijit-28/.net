using System;
using System.Collections.Generic;
using System.Text;

namespace Vehicle
{
    public interface IEngine
    {
        void Start();
    }

    // 2. Engine class ne interface ko implement kiya
    public class Engine : IEngine
    {
        public Engine(string eng)
        {
            Console.WriteLine($"Engine name is - {eng}");        
        }
        public void Start() => Console.WriteLine("Engine started... Vroom!");
    }
}
