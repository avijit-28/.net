
using Microsoft.Extensions.Configuration;
using Assignment3.Configuration;
using Assignment3.Repositories;

// Load appsettings.json from the application output directory.
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Load application settings.
AppSettings settings = AppSettings.Load(configuration);
MeterReconciliationRepository repository =

    new MeterReconciliationRepository(settings);

await repository.TestConnectionAsync();

Console.WriteLine("Configuration loaded successfully.");
Console.WriteLine($"API Base URL: {settings.ApiBaseUrl}");
Console.WriteLine($"Timeout: {settings.TimeoutSeconds} seconds");
Console.WriteLine($"Maximum retries: {settings.MaxRetryAttempts}");

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
