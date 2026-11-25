
using System.Data;
using Microsoft.Extensions.Configuration;
using Assignment3.Configuration;
using Assignment3.Repositories;
using Assignment3.ApiClients;
using Assignment3.Services;

// Load appsettings.json.
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .Build();

// Load settings.
AppSettings settings = AppSettings.Load(configuration);

// Initialize components.
var repository =
    new MeterReconciliationRepository(settings);

var apiClient =
    new MeterApiClient(settings);

var orchestrator =
    new ReconciliationOrchestrator(
        apiClient,
        repository);

bool running = true;

while (running)
{
    Console.WriteLine("\n================================");
    Console.WriteLine(" METER RECONCILIATION SYSTEM");
    Console.WriteLine("================================");
    Console.WriteLine("1. Test SQL connection");
    Console.WriteLine("2. Synchronize meter readings");
    Console.WriteLine("3. View reconciliation summary");
    Console.WriteLine("0. Exit");
    Console.Write("Select option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await repository.TestConnectionAsync();
            break;

        case "2":
            await orchestrator.SyncMeterReadingsAsync();
            break;

        case "3":
            DataTable summary =
                await repository.GetReconciliationSummaryAsync();

            if (summary.Rows.Count == 0)
            {
                Console.WriteLine("No reconciliation summary found.");
            }
            else
            {
                Console.WriteLine("\nReconciliation Summary:");

                foreach (DataColumn column in summary.Columns)
                {
                    Console.Write($"{column.ColumnName,-25}");
                }

                Console.WriteLine();

                foreach (DataRow row in summary.Rows)
                {
                    foreach (DataColumn column in summary.Columns)
                    {
                        Console.Write(
                            $"{row[column],-25}");
                    }

                    Console.WriteLine();
                }
            }

            break;

        case "0":
            running = false;
            Console.WriteLine("Application closed.");
            break;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}
