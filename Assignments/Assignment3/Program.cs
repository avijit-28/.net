using System;
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
    Console.WriteLine("  METER RECONCILIATION SYSTEM   ");
    Console.WriteLine("================================");
    Console.WriteLine("1. Synchronize meter readings");
    Console.WriteLine("2. View reconciliation summary");
    Console.WriteLine("3. View anomalous readings");
    Console.WriteLine("0. Exit");
    Console.Write("Select option: ");

    string? choice = Console.ReadLine();
    if (choice == null)
    {
        break;
    }

    try
    {
        switch (choice.Trim())
        {
            case "1":
                await orchestrator.SyncMeterReadingsAsync();
                break;

            case "2":
                DataTable summary =
                    await repository.GetReconciliationSummaryAsync();

                if (summary.Rows.Count == 0)
                {
                    Console.WriteLine("No reconciliation summary found.");
                }
                else
                {
                    Console.WriteLine("\n--- Reconciliation Summary ---");

                    foreach (DataColumn column in summary.Columns)
                    {
                        Console.Write($"{column.ColumnName,-30}");
                    }

                    Console.WriteLine();
                    Console.WriteLine(new string('-', summary.Columns.Count * 30));

                    foreach (DataRow row in summary.Rows)
                    {
                        foreach (DataColumn column in summary.Columns)
                        {
                            Console.Write($"{row[column],-30}");
                        }

                        Console.WriteLine();
                    }
                }

                break;

            case "4":
                var anomalies = await repository.GetAnomalousReadingsAsync();

                if (anomalies.Count == 0)
                {
                    Console.WriteLine("No anomalous readings found.");
                }
                else
                {
                    Console.WriteLine("\n--- Anomalous Readings ---");
                    Console.WriteLine($"{"ReadingId",-10} {"MeterSerial",-15} {"ReadingDate",-22} {"CumulKWh",-12} {"PrevCumul",-12} {"Status",-10}");
                    Console.WriteLine(new string('-', 85));

                    foreach (var a in anomalies)
                    {
                        string prevStr = a.PreviousCumulativeKWh.HasValue ? a.PreviousCumulativeKWh.Value.ToString("F3") : "N/A";
                        Console.WriteLine($"{a.ReadingId,-10} {a.MeterSerialNumber,-15} {a.ReadingDate:yyyy-MM-dd HH:mm:ss} {a.CumulativeKWh,10:F3}  {prevStr,10}  {a.ReadingStatus,-10}");
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
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] An error occurred: {ex.Message}");
        Console.ResetColor();
    }
}
