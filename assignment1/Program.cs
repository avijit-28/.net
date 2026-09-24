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
//Console.WriteLine("Method Overloading Example:");
//Console.WriteLine("Area Calculator");
//AreaCalculator areaCalculator = new AreaCalculator();
//double rectangleArea = areaCalculator.CalculateArea(5, 10); // Rectangle
//double circleArea = areaCalculator.CalculateArea(4); // circle

//Console.WriteLine($"Area of Rectangle: {rectangleArea}");
//Console.WriteLine($"Area of Circle: {circleArea}");

//Console.WriteLine("\nMethod Overriding Example:");
//Shape shape1 = new Circle();
//Shape shape2 = new polygon();

//shape1.Draw(); // Calls Circle's Draw method
//shape2.Draw(); // Calls Polygon's Draw method


//----------------------------------------------------question 7-----------------------------------------------------

//SmartDevice light = new SmartLight();

//light.TurnOn();
//Console.WriteLine($"Energy Consumption: {light.GetEnergyConsumption()} watts");
//light.TurnOff();

//SmartDevice thermostat = new SmartThermostat();
//thermostat.TurnOn();
//Console.WriteLine($"Energy Consumption: {thermostat.GetEnergyConsumption()} watts");
//thermostat.TurnOff();

//SmartDevice ac = new SmartAC();
//ac.TurnOn();
//Console.WriteLine($"Energy Consumption: {ac.GetEnergyConsumption()} watts");
//ac.TurnOff();


//----------------------------------------------------question 8-----------------------------------------------------

//Loan homeLoan = new HomeLoan(); 
//homeLoan.VerifyDocumentation();
//Console.WriteLine($"Interest Rate for Home Loan: {homeLoan.CalculateInterest()}%");
//Console.WriteLine($"Eligibility for Home Loan: {homeLoan.CheckEligibility()}");
//homeLoan.SanctionLoan();
//Console.WriteLine('\n');

//Loan carLoan = new CarLoan();
//carLoan.VerifyDocumentation();
//Console.WriteLine($"Interest Rate for Car Loan: {carLoan.CalculateInterest()}%");
//Console.WriteLine($"Eligibility for Car Loan: {carLoan.CheckEligibility()}");
//carLoan.SanctionLoan();
//Console.WriteLine('\n');

//Loan educationLoan = new EducationLoan();
//educationLoan.VerifyDocumentation();
//Console.WriteLine($"Interest Rate for Education Loan: {educationLoan.CalculateInterest()}%");
//Console.WriteLine($"Eligibility for Education Loan: {educationLoan.CheckEligibility()}");
//educationLoan.SanctionLoan();

//----------------------------------------------------question 9-----------------------------------------------------


//ICloudStorage googleStorage = new GoogleStorage();
//googleStorage.UploadFile("file1.txt");
//googleStorage.DownloadFile("file1.txt");
//googleStorage.DeleteFile("file1.txt");
//Console.WriteLine('\n');

//ICloudStorage azure_storage = new AzureStorage();
//azure_storage.UploadFile("file2.txt");
//azure_storage.DownloadFile("file2.txt");
//azure_storage.DeleteFile("file2.txt");

//----------------------------------------------------question 10-----------------------------------------------------

Warehouse robot = new Warehouse("BOT");
//robot.name = "WarehouseBot";
robot.Start();
robot.PerformTask();
robot.Detect();
robot.Navigate("Storage Area");
robot.Charge();

//Console.WriteLine("All operations completed successfully.");









