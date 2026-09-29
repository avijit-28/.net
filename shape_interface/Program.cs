using ConsoleApp1;

Console.WriteLine("enter the two shapes (Square , Rectangle)");
string shapeType =  Console.ReadLine();
shapeType = shapeType.ToLower();
//Console.WriteLine(shapeType);
IShape shape = ShapeFactory.getMyShape(shapeType);

if (shape != null)
{
    
    //float side1 = float.Parse(Console.ReadLine());
    //float side2 = float.Parse(Console.ReadLine());
    Console.WriteLine($"Area of {shapeType} is: {shape.Area()}");
    Console.WriteLine($"Perimeter of {shapeType} is: {shape.Perimeter()}");
}
else
{
    Console.WriteLine("Invalid shape type entered.");
}