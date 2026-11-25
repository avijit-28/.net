
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

// Load application settings.
AppSettings settings = AppSettings.Load(configuration);

// Initialize application components.
var repository =
    new MeterReconciliationRepository(
        settings.ConnectionString);

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

        case "0":
            running = false;
            Console.WriteLine("Application closed.");
            break;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}
