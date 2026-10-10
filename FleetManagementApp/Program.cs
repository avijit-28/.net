using System;
using System.Threading.Tasks;
using FleetManagementApp.Data;
using FleetManagementApp.Data.Repositories;
using FleetManagementApp.Services;
using FleetManagementApp.UI;

namespace FleetManagementApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Title = "Enterprise Fleet Management - ADO.NET";

                // 1. Initialize ADO.NET Data Helper (reads connection string from App.config)
                var sqlHelper = new SqlHelper();

                // 2. Initialize Repositories (Generic Repository Pattern)
                var vehicleRepository = new VehicleRepository(sqlHelper);

                // 3. Initialize Business Service Layer
                var fleetService = new FleetService(vehicleRepository);

                // 4. Initialize & Launch Console UI
                var menu = new ConsoleMenu(fleetService);
                await menu.RunAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[FATAL ERROR] {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[DETAILS] {ex.InnerException.Message}");
                }
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}