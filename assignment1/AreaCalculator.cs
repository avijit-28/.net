using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    internal class AreaCalculator
    {
        // Method overloading to calculate area of different shapes
        // Rectangle: length * width
        public double CalculateArea(double length, double width)
        {
            return length * width;
        }
        // Circle: π * radius^2
        public double CalculateArea(double radius)
        {
            return (22/7) * radius * radius;
        }
        
    }
    class Shape
    {
        public virtual void Draw()
        {
            Console.WriteLine("Drawing a shape.");
        }
    }

    class Circle : Shape
    {
        public override void Draw()
        {
            Console.WriteLine("Drawing a circle.");
        }
    }

    class polygon : Shape
    {
        public override void Draw()
        {
            Console.WriteLine("Drawing a polygon.");
        }
    }


}
