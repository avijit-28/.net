using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleApp1
{
    public interface IShape
    {
        float Area();

        float Perimeter();
    }
    public class ShapeFactory
    {
        public static IShape getMyShape(string shapeType)
        {
            IShape MyShape = null;
            Console.WriteLine("Enter the sides of the shape");

            if (shapeType == "square")
            {
                MyShape = new square(float.Parse(Console.ReadLine()));
            }
            else if (shapeType == "rectangle")
            {
                MyShape = new rectangle(float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()));
            }

            return MyShape;
        }
    }

    

    public class square: IShape
    {
        //float side1, side2;
        //public float Area(float a, float b)
        //{
        //    this.side1 = a;
        //    this.side2 = b;
        //    return side1 * side2;
        //}
        private float side;
        public square(float side)
        {
            this.side = side;

        }
        public float Area()
        {
            return side * side;
        }

        public float Perimeter()
        {
            return 4 * side;
        }
    }

    public class rectangle : IShape
    {
        private float side1, side2;

        public rectangle(float side1, float side2)
        {
            this.side1 = side1;
            this.side2 = side2;
        }
        public float Area()
        {
            return side1 * side2;
        }
        public float Perimeter()
        {
            return 2 * (side1 + side2);
        }
    }


}
