using System;

namespace Assignment3
{
    class Program
    {
        static void Main(string[] args)
        {
            // =====================================================
            // Secure Payment Processor Module Demo
            // =====================================================
            Console.WriteLine("=== SECURE PAYMENT PROCESSOR MODULE ===\n");
            PaymentProcessor gateway = new PaymentProcessor();

            // 1. Testing Balance Validation (Cannot be negative)
            Console.WriteLine("--- Test 1: Account Balance Encapsulation ---");
            gateway.AccountBalance = -500; // Rejected!
            gateway.AccountBalance = 25000; // Accepted!
            Console.WriteLine($"Current Balance: ${gateway.AccountBalance}\n");

            // 2. Testing Card Masking
            Console.WriteLine("--- Test 2: Card Masking Encapsulation ---");
            gateway.CardNumber = "12345"; // Rejected (not 16 digits)
            gateway.CardNumber = "4532119988773456"; // Accepted
            // Reading card number returns masked value:
            Console.WriteLine($"Masked Card Display: {gateway.CardNumber}\n");

            // 3. Testing Write-Only Security Token
            Console.WriteLine("--- Test 3: Write-Only Token Encapsulation ---");
            gateway.SecurityToken = "BANK-OAUTH-TOKEN-XY99882"; 
            Console.WriteLine();

            // 4. Processing Payment
            Console.WriteLine("--- Test 4: Processing Payment ---");
            gateway.ProcessTransaction(7500); // Success
            gateway.ProcessTransaction(30000); // Declined (Insufficient funds)
            Console.WriteLine();

            // =====================================================
            // Healthcare Patient Records Demo
            // =====================================================
            Healthcare patient = new Healthcare(101, "Rahul Verma");
            Console.WriteLine($"Patient Registered: {patient.PatientName} (ID: {patient.PatientId})\n");
            patient.SetMedicalHistory("Patient has asthma and mild seasonal allergy.");
            Console.WriteLine($"Medical History: {patient.GetMedicalHistory()}\n");

            Console.WriteLine("--- Temperature Validation Test ---");
            patient.SetTemperature(48.5); // Invalid
            patient.SetTemperature(37.5); // Valid

            Console.WriteLine("--- Billing Status Test ---");
            Console.WriteLine($"Initial Bill Status: Paid = {patient.BillPaid}");
            patient.payBill();
            Console.WriteLine($"Updated Bill Status: Paid = {patient.BillPaid}");
            patient.payBill(); // Should indicate already paid
            Console.WriteLine();

            // =====================================================
            // Enterprise Payroll System Demo
            // =====================================================
            employee baseEmp = new employee(100, "Suresh Kumar", 30000);
            FullTimeEmployee ftEmp = new FullTimeEmployee(101, "Aarav Sharma", 60000, 10000, 5000);
            ContractEmployee ctEmp = new ContractEmployee(102, "Pooja Verma", 45000, 6, 4000);
            FreelanceEmployee flEmp = new FreelanceEmployee(103, "Rohan Das", 15000, 8000, 5);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    ENTERPRISE PAYROLL SYSTEM MENU      ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Base Employee Info");
                Console.WriteLine("2. Full-Time Employee Payroll");
                Console.WriteLine("3. Contract Employee Payroll");
                Console.WriteLine("4. Freelance Employee Payroll");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter choice (0-4): ");
                string choice = Console.ReadLine();
                if (choice == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Exiting Payroll System. Goodbye!");
                    break;
                }
                Console.Clear();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("=== [BASE EMPLOYEE] ===");
                        baseEmp.DisplayEmployeeInfo();
                        break;
                    case "2":
                        Console.WriteLine("=== [FULL-TIME EMPLOYEE] ===");
                        ftEmp.DisplayEmployeeInfo();
                        break;
                    case "3":
                        Console.WriteLine("=== [CONTRACT EMPLOYEE] ===");
                        ctEmp.DisplayEmployeeInfo();
                        break;
                    case "4":
                        Console.WriteLine("=== [FREELANCE EMPLOYEE] ===");
                        flEmp.DisplayEmployeeInfo();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please select 0-4.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
