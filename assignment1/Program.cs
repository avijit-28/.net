using System;
using System.Collections.Generic;
using assignment1;

// ==============================================================================================
//                              C# OOP ASSIGNMENT 1 - MASTER RUNNER                              
// ==============================================================================================

while (true)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                    C# OBJECT ORIENTED PROGRAMMING ASSIGNMENT                 ║");
    Console.WriteLine("║                                MASTER CONSOLE                                ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    Console.WriteLine(" Select a Question to Execute:\n");
    Console.WriteLine("  [1]  Question 1  : Encapsulation — Secure Payment Processor Module");
    Console.WriteLine("  [2]  Question 2  : Encapsulation & Data Hiding — Healthcare Patient Records");
    Console.WriteLine("  [3]  Question 3  : Inheritance — Enterprise Employee Hierarchy & Payroll");
    Console.WriteLine("  [4]  Question 4  : Multilevel Inheritance — Vehicle Telematics & Fleet");
    Console.WriteLine("  [5]  Question 5  : Polymorphism — Dynamic Notification & Alerting Pipeline");
    Console.WriteLine("  [6]  Question 6  : Compile vs Runtime Polymorphism — Math & CAD Engine");
    Console.WriteLine("  [7]  Question 7  : Abstraction — IoT Smart Home Automation Framework");
    Console.WriteLine("  [8]  Question 8  : Abstract Classes — Banking Loan Processing Workflow");
    Console.WriteLine("  [9]  Question 9  : Interfaces — Plug-and-Play Cloud Storage Provider");
    Console.WriteLine("  [10] Question 10 : Abstract Class vs Interface — Autonomous Robot Engine");
    Console.WriteLine("────────────────────────────────────────────────────────────────────────────────");
    Console.WriteLine("  [0]  Exit Application");
    Console.WriteLine("────────────────────────────────────────────────────────────────────────────────");
    Console.Write("Enter your choice (0-10): ");

    string? choice = Console.ReadLine()?.Trim();

    if (choice == "0")
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nThank you! Exiting C# Assignment Console. Have a great day!\n");
        Console.ResetColor();
        break;
    }

    Console.Clear();

    switch (choice)
    {
        // =====================================================================================
        // QUESTION 1: Secure Payment Processor (Encapsulation)
        // =====================================================================================
        case "1":
            PrintHeader("QUESTION 1: SECURE PAYMENT PROCESSING MODULE");

            PaymentProcessor gateway = new PaymentProcessor();

            Console.WriteLine("--- Test 1: Account Balance Encapsulation ---");
            Console.WriteLine("Attempting to set negative balance (-$500)...");
            gateway.AccountBalance = -500; // Validation rejection
            Console.WriteLine("Setting valid balance ($25,000)...");
            gateway.AccountBalance = 25000;
            Console.WriteLine($"Current Balance: ${gateway.AccountBalance:N2}\n");

            Console.WriteLine("--- Test 2: Card Masking Encapsulation ---");
            Console.WriteLine("Attempting invalid card length ('12345')...");
            gateway.CardNumber = "12345"; // Validation rejection
            Console.WriteLine("Setting valid 16-digit card ('4532119988773456')...");
            gateway.CardNumber = "4532119988773456";
            Console.WriteLine($"Masked Card Display: {gateway.CardNumber}\n");

            Console.WriteLine("--- Test 3: Write-Only Security Token ---");
            gateway.SecurityToken = "BANK-OAUTH-TOKEN-XY99882";
            Console.WriteLine("Security token configured. (Protected from public read access)\n");

            Console.WriteLine("--- Test 4: Processing Payment Transactions ---");
            gateway.ProcessTransaction(7500);  // Success
            gateway.ProcessTransaction(30000); // Declined
            break;

        // =====================================================================================
        // QUESTION 2: Healthcare Patient Record (Data Hiding)
        // =====================================================================================
        case "2":
            PrintHeader("QUESTION 2: HEALTHCARE PATIENT RECORDS SYSTEM");

            Healthcare patient = new Healthcare(101, "Rahul Verma");
            Console.WriteLine($"Patient Registered: {patient.PatientName} (ID: {patient.PatientId})\n");

            Console.WriteLine("--- Medical History ---");
            patient.SetMedicalHistory("Patient has asthma and mild seasonal allergy.");
            Console.WriteLine($"Medical History: {patient.GetMedicalHistory()}\n");

            Console.WriteLine("--- Temperature Validation Test ---");
            Console.WriteLine("Setting invalid temperature (48.5°C)...");
            patient.SetTemperature(48.5); // Rejection
            Console.WriteLine("Setting valid temperature (37.5°C)...");
            patient.SetTemperature(37.5);
            Console.WriteLine($"Recorded Temperature: {patient.GetTemperature()}°C\n");

            Console.WriteLine("--- Billing Status Encapsulation ---");
            Console.WriteLine($"Initial Bill Paid Status: {patient.BillPaid}");
            patient.payBill();
            Console.WriteLine($"Updated Bill Paid Status: {patient.BillPaid}");
            Console.WriteLine("Attempting duplicate payment:");
            patient.payBill();
            break;

        // =====================================================================================
        // QUESTION 3: Employee Hierarchy & Payroll (Inheritance)
        // =====================================================================================
        case "3":
            PrintHeader("QUESTION 3: ENTERPRISE EMPLOYEE HIERARCHY & PAYROLL");

            employee baseEmp = new employee(100, "Suresh Kumar", 30000);
            FullTimeEmployee ftEmp = new FullTimeEmployee(101, "Aarav Sharma", 60000, 10000, 5000);
            ContractEmployee ctEmp = new ContractEmployee(102, "Pooja Verma", 45000, 6, 4000);
            FreelanceEmployee flEmp = new FreelanceEmployee(103, "Rohan Das", 15000, 8000, 5);

            Console.WriteLine("1. [BASE EMPLOYEE]");
            baseEmp.DisplayEmployeeInfo();
            Console.WriteLine("\n2. [FULL-TIME EMPLOYEE]");
            ftEmp.DisplayEmployeeInfo();
            Console.WriteLine("\n3. [CONTRACT EMPLOYEE]");
            ctEmp.DisplayEmployeeInfo();
            Console.WriteLine("\n4. [FREELANCE EMPLOYEE]");
            flEmp.DisplayEmployeeInfo();
            break;

        // =====================================================================================
        // QUESTION 4: Vehicle Telematics (Multilevel Inheritance)
        // =====================================================================================
        case "4":
            PrintHeader("QUESTION 4: VEHICLE TELEMATICS & FLEET MANAGEMENT");

            Vehicle v1 = new Vehicle("V-001", "DL-01-AA-1111", 28.7041, 77.1025, "SEC-01");
            MotorizedVehicle v2 = new MotorizedVehicle("MV-002", "DL-02-BB-2222", 28.5355, 77.3910, "SEC-02", 150, 60);
            ElectricVehicle v3 = new ElectricVehicle("EV-003", "DL-03-CC-3333", 19.0760, 72.8777, "SEC-03", 450, 80);

            Console.WriteLine("--- LEVEL 1: BASE VEHICLE ---");
            v1.LogGPS();
            v1.StartEngine();

            Console.WriteLine("\n--- LEVEL 2: MOTORIZED VEHICLE ---");
            v2.LogGPS();
            v2.StartEngine();
            v2.UpdateLocation(28.6000, 77.2000);

            Console.WriteLine("\n--- LEVEL 3: ELECTRIC VEHICLE ---");
            v3.LogGPS();
            v3.StartEngine();
            v3.calculateRange();
            v3.UpdateLocation(19.2000, 72.9000);
            break;

        // =====================================================================================
        // QUESTION 5: Notification Pipeline (Late Binding Polymorphism)
        // =====================================================================================
        case "5":
            PrintHeader("QUESTION 5: DYNAMIC NOTIFICATION & ALERTING PIPELINE");

            List<Notification> alertingPipeline = new List<Notification>
            {
                new EmailNotification(),
                new SMSNotification(),
                new WhatsAppNotification(),
                new PushNotification()
            };

            DispatchAllAlerts(alertingPipeline, "CRITICAL: Database CPU usage exceeded 90%!");
            break;

        // =====================================================================================
        // QUESTION 6: Math & CAD Engine (Compile vs Runtime Polymorphism)
        // =====================================================================================
        case "6":
            PrintHeader("QUESTION 6: MATH ENGINE & GEOMETRY ENGINE");

            Console.WriteLine("--- COMPILE-TIME POLYMORPHISM (Method Overloading) ---");
            AreaCalculator areaCalculator = new AreaCalculator();
            double rectArea = areaCalculator.CalculateArea(5.0, 10.0);
            double circArea = areaCalculator.CalculateArea(4.0);
            double triArea = areaCalculator.CalculateArea(6.0, 8.0, true);

            Console.WriteLine($"Rectangle Area (5 x 10)    : {rectArea}");
            Console.WriteLine($"Circle Area (Radius 4)     : {circArea:F2}");
            Console.WriteLine($"Triangle Area (Base 6, H 8): {triArea}\n");

            Console.WriteLine("--- RUNTIME POLYMORPHISM (Method Overriding / Late Binding) ---");
            List<Shape> cadCanvas = new List<Shape>
            {
                new Circle(7.5),
                new Polygon(6)
            };

            foreach (Shape shape in cadCanvas)
            {
                shape.Draw();
            }
            break;

        // =====================================================================================
        // QUESTION 7: Smart Home Automation (Abstraction)
        // =====================================================================================
        case "7":
            PrintHeader("QUESTION 7: SMART HOME AUTOMATION FRAMEWORK");

            List<SmartDevice> homeDevices = new List<SmartDevice>
            {
                new SmartLight("Living Room Light"),
                new SmartThermostat("Hallway Thermostat"),
                new SmartAC("Master Bedroom AC")
            };

            Console.WriteLine("1. Activating All Devices:");
            foreach (SmartDevice device in homeDevices)
            {
                device.TurnOn();
            }

            Console.WriteLine("\n2. Energy Audit:");
            double totalPower = 0;
            foreach (SmartDevice device in homeDevices)
            {
                double power = device.GetEnergyConsumption();
                Console.WriteLine($" - {device.DeviceName,-22} : {power,6} W");
                totalPower += power;
            }
            Console.WriteLine($" Total Power Consumption : {totalPower} W ({totalPower / 1000:F2} kW)\n");

            Console.WriteLine("3. Shutting Down Devices:");
            foreach (SmartDevice device in homeDevices)
            {
                device.TurnOff();
            }
            break;

        // =====================================================================================
        // QUESTION 8: Banking Loan Processing (Abstract Classes & Template Pattern)
        // =====================================================================================
        case "8":
            PrintHeader("QUESTION 8: BANKING LOAN PROCESSING WORKFLOW");

            Loan homeLoan = new HomeLoan("Rahul Sharma", 5000000);
            homeLoan.ProcessLoan();

            Loan carLoan = new CarLoan("Priya Verma", 1200000);
            carLoan.ProcessLoan();

            Loan eduLoan = new EducationLoan("Aman Gupta", 400000);
            eduLoan.ProcessLoan();
            break;

        // =====================================================================================
        // QUESTION 9: Cloud Storage Provider (Interfaces & DIP)
        // =====================================================================================
        case "9":
            PrintHeader("QUESTION 9: STORAGE SYSTEM");

            Console.WriteLine("--- 1. Using Amazon S3 Provider ---");
            ICloudStorageProvider storage = new S3Storage();
            storage.UploadFile("user_video.mp4");
            storage.DownloadFile("VID-101");

            Console.WriteLine("\n--- 2. Switching to Azure Blob Provider (Plug-and-Play) ---");
            storage = new AzureStorage();
            storage.UploadFile("profile_picture.png");
            storage.DownloadFile("IMG-502");

            Console.WriteLine("\n--- 3. Switching to Google Cloud Storage ---");
            storage = new GoogleCloudStorage();
            storage.DeleteFile("OLD-ARCHIVE-99");
            break;

        // =====================================================================================
        // QUESTION 10: Autonomous Robot Engine (Abstract Class vs Interface)
        // =====================================================================================
        case "10":
            PrintHeader("QUESTION 10: AUTONOMOUS ROBOT NAVIGATION ENGINE");

            WarehouseRobot robot = new WarehouseRobot("Neon-AGV");

            Console.WriteLine("--- Base Class Identity & Workflow ---");
            robot.Start();
            robot.PerformTask();

            Console.WriteLine("\n--- Interface Capabilities (Multiple Interfaces) ---");
            robot.Detect();
            robot.Navigate("Storage Bay 4, Aisle B");
            robot.Charge("docking and charging battery");

            Console.WriteLine("\n--- Interface Polymorphism Reference Checks ---");
            INavigable navRef = robot;
            navRef.Navigate("Charging Station Alpha");

            IChargeable chargeRef = robot;
            chargeRef.Charge("fully charged and ready");
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid selection! Please enter a number from 0 to 10.");
            Console.ResetColor();
            break;
    }

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("\n────────────────────────────────────────────────────────────────────────────────");
    Console.ResetColor();
    Console.WriteLine("Press any key to return to Master Menu...");
    Console.ReadKey();
}

// ==============================================================================================
//                                     HELPER METHODS                                            
// ==============================================================================================

static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine($"║ {title,-76} ║");
    //Console.WriteLine($"║ Concept: {subtitle,-67} ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝\n");
    Console.ResetColor();
}

static void DispatchAllAlerts(List<Notification> pipeline, string message)
{
    Console.WriteLine($"=== DISPATCHING ALERT PIPELINE VIA LATE BINDING ===");
    Console.WriteLine($"Payload: \"{message}\"\n");
    foreach (Notification channel in pipeline)
    {
        channel.SendNotification(message);
    }
}