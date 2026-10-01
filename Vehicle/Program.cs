//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Vehicle; // Aapki classes ka namespace

//// 1. DI Container (Builder) taiyar karein
//var builder = Host.CreateApplicationBuilder(args);

//// 2. Apni dependencies ko register karein
//builder.Services.AddTransient<IEngine, Engine>(); // Jab IEngine mange toh Engine dena
//builder.Services.AddTransient<Car>();             // Car ko bhi register kiya

//// 3. Application ko build karein
//using var host = builder.Build();

//// 4. Container se Car ka instance maangein (DI background me khud Engine inject kar dega)
//var myCar = host.Services.GetRequiredService<Car>();

//// 5. Output check karne ke liye run karein
//myCar.Drive();


using Vehicle;

IEngine eng = new Engine("V8");

Car c1 = new Car();
//c1.Drive();
//c1.Engine = eng;
//c1.SetEngine(eng);
//c1.Engine.Start();
//c1.getEngine().Start();
//var c = c1.getEngine();
//c.Start();

c1.StartEngine(eng);

Console.ReadLine();