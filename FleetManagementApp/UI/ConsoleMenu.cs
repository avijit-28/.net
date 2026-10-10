using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FleetManagementApp.Models;
using FleetManagementApp.Services;

namespace FleetManagementApp.UI
{
    public class ConsoleMenu
    {
        private readonly FleetService _fleetService;

        public ConsoleMenu(FleetService fleetService)
        {
            _fleetService = fleetService ?? throw new ArgumentNullException(nameof(fleetService));
        }

        public async Task RunAsync()
        {
            bool exit = false;

            while (!exit)
            {
                DisplayHeader();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  1. [Create] Onboard New Vehicle");
                Console.WriteLine("  2. [Read]   View Active Fleet Inventory");
                Console.WriteLine("  3. [Read]   View All Vehicles");
                Console.WriteLine("  4. [Read]   Find Vehicle by ID");
                Console.WriteLine("  5. [Update] Log Telemetry (Odometer & Status)");
                Console.WriteLine("  6. [Delete] Decommission Vehicle");
                Console.WriteLine("  7. Exit Application");
                Console.ResetColor();
                Console.Write("\nSelect an option (1-7): ");

                string choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await OnboardVehiclePromptAsync();
                        break;
                    case "2":
                        await ViewActiveFleetAsync();
                        break;
                    case "3":
                        await ViewAllVehiclesAsync();
                        break;
                    case "4":
                        await FindVehicleByIdPromptAsync();
                        break;
                    case "5":
                        await LogTelemetryPromptAsync();
                        break;
                    case "6":
                        await DecommissionVehiclePromptAsync();
                        break;
                    case "7":
                        exit = true;
                        Console.WriteLine("Exiting Fleet Management System. Goodbye!");
                        break;
                    default:
                        PrintError("Invalid choice! Please select an option between 1 and 7.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
            }
        }

        private void DisplayHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("================================================================================");
            Console.WriteLine("                   ENTERPRISE FLEET MANAGEMENT SYSTEM (ADO.NET)                ");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
        }

        private async Task OnboardVehiclePromptAsync()
        {
            Console.WriteLine("--- [FLEET ONBOARDING] Add New Vehicle ---");

            Console.Write("Enter VIN (17 characters): ");
            string vin = Console.ReadLine()?.Trim().ToUpper();

            Console.Write("Enter Make (e.g. Volvo, Freightliner): ");
            string make = Console.ReadLine()?.Trim();

            Console.Write("Enter Model (e.g. VNL 860, Cascadia): ");
            string model = Console.ReadLine()?.Trim();

            Console.Write("Enter Year: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                PrintError("Invalid year entered.");
                return;
            }

            Console.Write("Enter Initial Odometer Reading: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal odometer))
            {
                PrintError("Invalid odometer reading.");
                return;
            }

            var vehicle = new Vehicle(vin, make, model, year, odometer);
            var result = await _fleetService.OnboardVehicleAsync(vehicle);

            if (result.Success)
            {
                PrintSuccess($"{result.Message} Generated Vehicle ID: {result.VehicleId}");
            }
            else
            {
                PrintError(result.Message);
            }
        }

        private async Task ViewActiveFleetAsync()
        {
            Console.WriteLine("--- [INVENTORY] Real-Time Active Fleet ---");
            var vehicles = await _fleetService.GetActiveFleetInventoryAsync();
            RenderVehicleTable(vehicles);
        }

        private async Task ViewAllVehiclesAsync()
        {
            Console.WriteLine("--- [INVENTORY] All Fleet Vehicles ---");
            var vehicles = await _fleetService.GetAllVehiclesAsync();
            RenderVehicleTable(vehicles);
        }

        private async Task FindVehicleByIdPromptAsync()
        {
            Console.WriteLine("--- [QUERY] Find Vehicle By ID ---");
            Console.Write("Enter Vehicle ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                PrintError("Invalid numeric ID.");
                return;
            }

            var vehicle = await _fleetService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                PrintError($"No vehicle found with ID: {id}");
                return;
            }

            Console.WriteLine();
            RenderVehicleTable(new List<Vehicle> { vehicle });
        }

        private async Task LogTelemetryPromptAsync()
        {
            Console.WriteLine("--- [TELEMETRY LOGGING] Update Mileage & Operational Status ---");

            Console.Write("Enter Vehicle ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                PrintError("Invalid ID format.");
                return;
            }

            var vehicle = await _fleetService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                PrintError($"Vehicle with ID {id} not found.");
                return;
            }

            Console.WriteLine($"Current details: {vehicle.Make} {vehicle.Model} | Mileage: {vehicle.OdometerReading:N1} km | Status: {vehicle.Status}");

            Console.Write($"Enter New Odometer Reading (must be >= {vehicle.OdometerReading:N1}): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal newOdometer))
            {
                PrintError("Invalid mileage format.");
                return;
            }

            Console.WriteLine("\nSelect New Operational Status:");
            Console.WriteLine("1. Active");
            Console.WriteLine("2. InTransit");
            Console.WriteLine("3. Maintenance");
            Console.WriteLine("4. Decommissioned");
            Console.Write("Choice (1-4): ");

            VehicleStatus newStatus = vehicle.Status;
            switch (Console.ReadLine()?.Trim())
            {
                case "1": newStatus = VehicleStatus.Active; break;
                case "2": newStatus = VehicleStatus.InTransit; break;
                case "3": newStatus = VehicleStatus.Maintenance; break;
                case "4": newStatus = VehicleStatus.Decommissioned; break;
                default:
                    PrintError("Invalid status selection.");
                    return;
            }

            var result = await _fleetService.LogTelemetryAsync(id, newOdometer, newStatus);
            if (result.Success)
            {
                PrintSuccess(result.Message);
            }
            else
            {
                PrintError(result.Message);
            }
        }

        private async Task DecommissionVehiclePromptAsync()
        {
            Console.WriteLine("--- [DECOMMISSIONING] Retire Vehicle Record ---");
            Console.Write("Enter Vehicle ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                PrintError("Invalid ID format.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Are you sure you want to permanently delete vehicle ID {id}? (y/n): ");
            Console.ResetColor();

            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                Console.WriteLine("Decommissioning aborted.");
                return;
            }

            var result = await _fleetService.DecommissionVehicleAsync(id);
            if (result.Success)
            {
                PrintSuccess(result.Message);
            }
            else
            {
                PrintError(result.Message);
            }
        }

        private void RenderVehicleTable(IReadOnlyList<Vehicle> vehicles)
        {
            if (vehicles == null || vehicles.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("No records found.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("-----------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("{0,-5} | {1,-17} | {2,-12} | {3,-15} | {4,-5} | {5,12} | {6,-12}",
                "ID", "VIN", "MAKE", "MODEL", "YEAR", "ODOMETER", "STATUS"));
            Console.WriteLine("-----------------------------------------------------------------------------------------");

            foreach (var v in vehicles)
            {
                Console.WriteLine(string.Format("{0,-5} | {1,-17} | {2,-12} | {3,-15} | {4,-5} | {5,12:N1} | {6,-12}",
                    v.Id, v.VIN, v.Make, v.Model, v.Year, v.OdometerReading, v.Status));
            }
            Console.WriteLine("-----------------------------------------------------------------------------------------");
            Console.WriteLine($"Total Vehicles: {vehicles.Count}");
        }

        private void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCCESS] {message}");
            Console.ResetColor();
        }

        private void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ResetColor();
        }
    }
}