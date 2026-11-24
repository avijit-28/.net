using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    public class AreaCalculator
    {
        // Method overloading to calculate area of different shapes
        
        public double CalculateArea(double length, double width)
        {
            return length * width;
        }
        
        public double CalculateArea(double radius)
        {
            return (22/7) * radius * radius;
        }
        public double CalculateArea(double baseLength, double height, bool isTriangle)
        {
            return 0.5 * baseLength * height;
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
        public double Radius { get; set; }
        public Circle(double radius)
        {
            Radius = radius;

        }
        public override void Draw()
        {
            Console.WriteLine($"Drawing a circle of radius {Radius}.");
        }
    }

    class Polygon : Shape
    {
        public int NumberOfSides { get; set; }
        public Polygon(int numberOfSides)
        {
            NumberOfSides = numberOfSides;
        }
        public override void Draw()
        {
            Console.WriteLine($"Drawing a polygon with {NumberOfSides} sides.");
        }
    }


}
