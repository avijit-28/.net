
using Microsoft.Extensions.Configuration;
using Assignment3.Configuration;
using Assignment3.Repositories;

// Load appsettings.json from the application output directory.
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .Build();

// Load application settings.
AppSettings settings = AppSettings.Load(configuration);

// Create repository using the database connection string.
MeterReconciliationRepository repository =
    new MeterReconciliationRepository(
        settings.ConnectionString);

// Test SQL connection.
await repository.TestConnectionAsync();

Console.WriteLine("Configuration loaded successfully.");
Console.WriteLine($"API Base URL: {settings.ApiBaseUrl}");
Console.WriteLine($"Timeout: {settings.TimeoutSeconds} seconds");
Console.WriteLine($"Maximum retries: {settings.MaxRetryAttempts}");

if (!Console.IsInputRedirected)
{
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}
