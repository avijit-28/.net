
using MethodOverriding;

Employee emp1 = new Developer()
{
    Id = 1,
    Name = "John Doe",
    Designation = "Software Developer",
    Salary = 40000
};
Console.WriteLine(emp1);
double bonus = emp1.CalculateBonus(emp1.Salary);
Console.WriteLine(bonus);

Employee emp2 = new Manager()
{
    Id = 2,
    Name = "Jane Smith",
    Designation = "GM",
    Salary = 50000
};

Console.WriteLine(emp2);
bonus = emp2.CalculateBonus(emp2.Salary);
Console.WriteLine(bonus);

Employee emp3 = new Admin()
{
    Id = 3,
    Name = "Avijit",
    Designation = "admin",
    Salary = 55000
};
Console.WriteLine(emp3);
bonus =  emp3.CalculateBonus(emp3.Salary);
Console.WriteLine(bonus);