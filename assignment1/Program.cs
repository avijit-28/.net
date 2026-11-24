using assignment1;


//----------------------------------------------------question 1-----------------------------------------------------

//PaymentProcessor payment = new PaymentProcessor();

//payment.CardNumber = "9876543215935795";
//payment.AccountBalance = 1000.00m;
//payment.SecurityToken = "securetoken123";

//Console.WriteLine($"Card Number: {payment.CardNumber}");
//Console.WriteLine($"Account Balance: {payment.AccountBalance}");


//----------------------------------------------------question 2-----------------------------------------------------

//Healthcare patient = new Healthcare(1, "John Doe");
//patient.SetMedicalHistory("No known allergies. Previous surgery in 2015.");
//patient.SetTemperature(36.5);

//patient.payBill();

//Console.WriteLine($"Patient ID: {patient.PatientId}");
//Console.WriteLine($"Patient Name: {patient.PatientName}");
//Console.WriteLine($"Medical History: {patient.GetMedicalHistory()}");
//Console.WriteLine($"Temperature: {patient.GetTemperature()} °C");
//Console.WriteLine($"Bill Paid: {patient.BillPaid}");

//----------------------------------------------------question 3-----------------------------------------------------

//FullTimeEmployee emp1 = new FullTimeEmployee(1, "Alice", 50000, 5000);
//ContractEmployee emp2 = new ContractEmployee(2, "Bob", 30000);
//FreelanceEmployee emp3 = new FreelanceEmployee(3, "Charlie", 20000, 3000);

//Console.WriteLine("Full-Time Employee:");
//emp1.DisplayEmployeeInfo();
//Console.WriteLine($"Net Salary: {emp1.CalculateNetSalary()}");

//Console.WriteLine("\nContract Employee:");
//emp2.DisplayEmployeeInfo();
//Console.WriteLine($"Net Salary: {emp2.CalculateNetSalary()}");

//Console.WriteLine("\nFreelance Employee:");
//emp3.DisplayEmployeeInfo();
//Console.WriteLine($"Net Salary: {emp3.CalculateNetSalary()}");

//----------------------------------------------------question 4-----------------------------------------------------


//ElectricVehicle ev = new ElectricVehicle(80); // Battery status is 80%
//ev.VehicleNumber = "EV-2003";
//ev.LogGPS();
//ev.StartEngine();
//ev.calculateRange();

//const string message = "Electric vehicle operations completed successfully.";
//Console.WriteLine(message);


//----------------------------------------------------question 5-----------------------------------------------------


//List<Notification> notifications = new List<Notification>
//{
//    new EmailNotification(),
//    new SMSNotification(),
//    new PushNotification(),
//    new whatsappNotification()
// };
//foreach (Notification notification in notifications)
//{
//notification.SendNotification("Sending Message Successful");fg 
//}

//----------------------------------------------------question 6-----------------------------------------------------
Console.WriteLine("Method Overloading Example:");
Console.WriteLine("Area Calculator");

AreaCalculator areaCalculator = new AreaCalculator();
double rectangleArea = areaCalculator.CalculateArea(5, 10); // Rectangle
double circleArea = areaCalculator.CalculateArea(4); // circle
double triangleArea = areaCalculator.CalculateArea(6.0, 8.0, true);

Console.WriteLine($"Area of Rectangle: {rectangleArea}");
Console.WriteLine($"Area of Circle: {circleArea}");
Console.WriteLine($"Triangle Area: {triangleArea}");

Console.WriteLine("\nMethod Overriding Example:");
List<Shape> cadCanvas = new List<Shape>
            {
                new Circle(7.5),
                new Polygon(),      // Hexagon
                
            };
// At runtime, the CLR checks the actual object type and dispatches the correct Draw()
foreach (Shape shape in cadCanvas)
{
    shape.Draw();
}


//----------------------------------------------------question 7-----------------------------------------------------

//List<SmartDevice> homeDevices = new List<SmartDevice>
//            {
//                new SmartLight("Living Room Light"),
//                new SmartThermostat("Hallway Thermostat"),
//                new SmartAC("Master Bedroom AC")
//            };
//while (true)
//{
//    Console.Clear(); // Clears console on main menu / when coming back
//    Console.WriteLine("==============================");
//    Console.WriteLine("    SMART HOME CONTROLLER     ");
//    Console.WriteLine("==============================");
//    Console.WriteLine("Select a Device:");
//    for (int i = 0; i < homeDevices.Count; i++)
//    {
//        Console.WriteLine($"{i + 1}. {homeDevices[i].DeviceName}");
//    }
//    Console.WriteLine("0. Exit App");
//    Console.Write("\nEnter choice (0-3): ");
//    string deviceChoice = Console.ReadLine();
//    if (deviceChoice == "0")
//    {
//        Console.Clear();
//        Console.WriteLine("Exiting Smart Home App. Goodbye!");
//        break;
//    }
//    if (int.TryParse(deviceChoice, out int index) && index >= 1 && index <= homeDevices.Count)
//    {
//        SmartDevice selectedDevice = homeDevices[index - 1];
//        Console.Clear(); // Clears screen when device is selected
//        bool stayInSubMenu = true;
//        while (stayInSubMenu)
//        {
//            Console.WriteLine($"\n--- [{selectedDevice.DeviceName}] Controls ---");
//            Console.WriteLine("1. Turn ON");
//            Console.WriteLine("2. Turn OFF");
//            Console.WriteLine("3. Check Energy Consumption");
//            Console.WriteLine("4. <-- Back to Device List");
//            Console.Write("Choose action (1-4): ");
//            string actionChoice = Console.ReadLine();
//            switch (actionChoice)
//            {
//                case "1":
//                    selectedDevice.TurnOn();
//                    break;
//                case "2":
//                    selectedDevice.TurnOff();
//                    break;
//                case "3":
//                    double power = selectedDevice.GetEnergyConsumption();
//                    Console.WriteLine($"Current Power Draw: {power} Watts");
//                    break;
//                case "4":
//                    stayInSubMenu = false; // Exits loop and Console.Clear() runs at top
//                    break;
//                default:
//                    Console.WriteLine("Invalid action! Please choose 1, 2, 3, or 4.");
//                    break;
//            }
//        }
//    }
//    else
//    {
//        Console.WriteLine("Invalid device choice! Press Enter to try again...");
//        Console.ReadLine();
//    }
//}

//----------------------------------------------------question 8-----------------------------------------------------

//Loan homeLoan = new HomeLoan("Rahul Sharma", 5000000);
//homeLoan.ProcessLoan();
//Loan carLoan = new CarLoan("Priya Verma", 1200000);
//carLoan.ProcessLoan();
//Loan eduLoan = new EducationLoan("Aman Gupta", 400000);
//eduLoan.ProcessLoan();

//----------------------------------------------------question 9-----------------------------------------------------



//ICloudStorageProvider storage = new S3Storage();
//storage.UploadFile("video1.mp4");
//storage.DownloadFile("VID-101");

//Console.WriteLine("-----------------------------");

//storage = new AzureStorage();
//storage.UploadFile("photo.png");
//storage.DownloadFile("IMG-502");

//Console.WriteLine("-----------------------------");

//storage = new GoogleCloudStorage();
//storage.DeleteFile("IMG-502");

//----------------------------------------------------question 10-----------------------------------------------------

//WarehouseRobot robot = new WarehouseRobot("Neon");
////robot.Name = "Neon";
//robot.Start();
//robot.PerformTask();
//robot.Detect();
//robot.Navigate("Storage Area");
//robot.Charge("docking and charging battery");

//INavigable nav = robot;
//nav.Navigate("Charging Dock Station 1");

//IChargeable charger = robot;
//charger.Charge("fully charged");

Console.WriteLine("All operations completed successfully.");
Console.ReadKey();









