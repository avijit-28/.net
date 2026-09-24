using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shape
{
    public interface IShape
    {
        float area( float a);
        float perimeter(float l, float b);

        //float Perimeter(float a, float b, float c);
    }
    internal class allShape
    {
        public static IShape GetMyShape(string shapeType)
        { IShape MyShape = null;
            if (shapeType == "Circle")
            {
                MyShape = new Square();
            }
            else if (shapeType == "Rectangle")
            {
                MyShape = new Rectangle();
            }
            else if (shapeType == "Triangle")
            {
                MyShape = new Triangle();
            }
            return MyShape;
        }
    }

    public class Square : IShape 
    {
       float area(float a)
        {
            return a * a;
        }
        float perimeter(float l, float b)
        {
            return 4 * l;
        }
    }
    public class Rectangle : IShape
    {         
        float area(float l)
        {
            return l * l;
        }
        float perimeter(float l, float b)
        {
            return 2 * (l + b);
        }
    }

    public class Triangle : IShape
    {   float area(float b)
        {
            return 0.5f * b * b;
        }
        float perimeter(float a, float b)
        {
            return a + a + b;
        }
    }
}
