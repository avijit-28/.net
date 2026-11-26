using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Assignment3.Configuration;
using Assignment3.Repositories;
using Assignment3.ApiClients;
using Assignment3.Services;
using Assignment3.Models;

// Configure Console Output
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Load appsettings.json
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .Build();

// Load typed settings
AppSettings settings = AppSettings.Load(configuration);

// Initialize system components
var repository = new MeterReconciliationRepository(settings);
var apiClient = new MeterApiClient(settings);
var orchestrator = new ReconciliationOrchestrator(apiClient, repository);

bool running = true;

// Initial clear on startup
if (!Console.IsInputRedirected)
{
    try { Console.Clear(); } catch { }
}

while (running)
{
    PrintHeader();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("  [✓] System Active & Ready");
    Console.ResetColor();
    Console.WriteLine($"      • Target Database : MeterReconciliationDB");
    Console.WriteLine($"      • Web API Base URL: {settings.ApiBaseUrl}");
    Console.WriteLine(new string('─', 64));

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                 OPERATIONAL CONTROL PANEL                    ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("  [1]  ⚡  Synchronize Meter Readings (Fetch API & Reconcile)");
    Console.WriteLine("  [2]  📊  View Batch Reconciliation History & Audit Summaries");
    Console.WriteLine("  [3]  ⚠️  View Flagged High-Variance & Anomalous Readings");
    Console.WriteLine("  [4]  💡  View Active Tariff Slabs (Poll Web API)");
    Console.WriteLine("  [5]  🚨  View Outage & Dispatch Alerts (Poll Web API)");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("  [0]  🚪  Exit Application");
    Console.ResetColor();
    Console.WriteLine(new string('─', 64));

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("  Select Action [0-5] ▶ ");
    Console.ResetColor();

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
                PauseAndClear();
                break;

            case "2":
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
                Console.WriteLine("│             RECONCILIATION RUN AUDIT SUMMARY                 │");
                Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
                Console.ResetColor();

                DataTable summary = await repository.GetReconciliationSummaryAsync();

                if (summary.Rows.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  [i] No reconciliation audit logs found in database.");
                    Console.ResetColor();
                }
                else
                {
                    // Print table headers
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"  {"AuditId",-8} {"BatchId",-26} {"Status",-10} {"CreatedAt",-22} {"Summary Details"}");
                    Console.WriteLine("  " + new string('─', 110));
                    Console.ResetColor();

                    foreach (DataRow row in summary.Rows)
                    {
                        string auditId = row["AuditId"]?.ToString() ?? "";
                        string batchId = row["BatchId"]?.ToString() ?? "N/A";
                        bool isSuccess = row["IsSuccess"] is bool b && b;
                        string dateStr = Convert.ToDateTime(row["CreatedAt"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string details = row["Summary"]?.ToString() ?? "";

                        Console.Write($"  {auditId,-8} {batchId,-26} ");

                        if (isSuccess)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"{"Success",-10} ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write($"{"Alerts",-10} ");
                        }
                        Console.ResetColor();

                        Console.WriteLine($"{dateStr,-22} {details}");
                    }
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("  " + new string('─', 110));
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  [✓] Retrieved {summary.Rows.Count} reconciliation run(s).");
                    Console.ResetColor();
                }

                PauseAndClear();
                break;

            case "3":
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
                Console.WriteLine("│             FLAGGED ANOMALOUS METER READINGS                 │");
                Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
                Console.ResetColor();

                var anomalies = await repository.GetAnomalousReadingsAsync();

                if (anomalies.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  [✓] Great! Zero anomalous meter readings detected in grid.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"  {"ReadingId",-10} {"MeterSerial",-16} {"Feeder",-10} {"ReadingDate",-22} {"Current KWh",-14} {"Previous KWh",-14} {"Alert Reason"}");
                    Console.WriteLine("  " + new string('─', 110));
                    Console.ResetColor();

                    foreach (var a in anomalies)
                    {
                        string prevStr = a.PreviousCumulativeKWh.HasValue ? a.PreviousCumulativeKWh.Value.ToString("F3") : "N/A";
                        string feeder = string.IsNullOrWhiteSpace(a.FeederId) ? "N/A" : a.FeederId;

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"  {a.ReadingId,-10} {a.MeterSerialNumber,-16} {feeder,-10} {a.ReadingDate:yyyy-MM-dd HH:mm:ss}  {a.CumulativeKWh,11:F3}   {prevStr,11}   ");
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Negative Consumption: Rollback/Reset]");
                        Console.ResetColor();
                    }
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("  " + new string('─', 110));
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  [!] Found {anomalies.Count} high-variance reading(s) requiring engineer inspection.");
                    Console.ResetColor();
                }

                PauseAndClear();
                break;

            case "4":
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
                Console.WriteLine("│             ACTIVE TARIFF SLABS & BILLING RATES              │");
                Console.WriteLine("│             (Endpoint: /api/v1/tariff-updates)               │");
                Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
                Console.ResetColor();

                Console.WriteLine("  Querying active tariff structures from Web API...");
                List<TariffUpdateDto>? tariffs = null;

                try
                {
                    tariffs = await apiClient.GetTariffUpdatesAsync();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  [!] Web API currently unreachable: {ex.Message}");
                    Console.Write("  Would you like to view cached/sample tariff slabs? (y/n): ");
                    Console.ResetColor();
                    string? ans = Console.ReadLine()?.Trim().ToLowerInvariant();
                    if (ans == "y" || ans == "yes")
                    {
                        tariffs = new List<TariffUpdateDto>
                        {
                            new() { SlabId = 1, FeederId = "FDR-01", SlabName = "Domestic Tier 1", MinUnits = 0m, MaxUnits = 100m, RatePerUnit = 4.50m },
                            new() { SlabId = 2, FeederId = "FDR-01", SlabName = "Domestic Tier 2", MinUnits = 101m, MaxUnits = 300m, RatePerUnit = 6.25m },
                            new() { SlabId = 3, FeederId = "FDR-02", SlabName = "Commercial Tier 1", MinUnits = 0m, MaxUnits = 500m, RatePerUnit = 8.50m }
                        };
                    }
                }

                if (tariffs != null && tariffs.Count > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"  {"SlabId",-8} {"FeederId",-12} {"Slab Category",-22} {"Range (kWh)",-18} {"Rate (₹/kWh)",-14}");
                    Console.WriteLine("  " + new string('─', 78));
                    Console.ResetColor();

                    foreach (var t in tariffs)
                    {
                        string range = t.MaxUnits.HasValue ? $"{t.MinUnits} - {t.MaxUnits}" : $"> {t.MinUnits}";
                        Console.WriteLine($"  {t.SlabId,-8} {t.FeederId,-12} {t.SlabName,-22} {range,-18} ₹{t.RatePerUnit,-13:F2}");
                    }

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("  " + new string('─', 78));
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  [✓] Successfully retrieved {tariffs.Count} tariff slab(s).");
                    Console.ResetColor();
                }

                PauseAndClear();
                break;

            case "5":
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
                Console.WriteLine("│             GRID OUTAGE & DISPATCH NOTIFICATIONS             │");
                Console.WriteLine("│             (Endpoint: /api/v1/dispatch-alerts)              │");
                Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
                Console.ResetColor();

                Console.WriteLine("  Querying grid dispatch alerts from Web API...");
                List<DispatchAlertDto>? alerts = null;

                try
                {
                    alerts = await apiClient.GetDispatchAlertsAsync();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  [!] Web API currently unreachable: {ex.Message}");
                    Console.Write("  Would you like to view cached/sample dispatch alerts? (y/n): ");
                    Console.ResetColor();
                    string? ans = Console.ReadLine()?.Trim().ToLowerInvariant();
                    if (ans == "y" || ans == "yes")
                    {
                        alerts = new List<DispatchAlertDto>
                        {
                            new() { AlertId = "ALT-001", FeederId = "FDR-01", Severity = "Low", Message = "Scheduled maintenance at Substation Alpha." },
                            new() { AlertId = "ALT-002", FeederId = "FDR-03", Severity = "High", Message = "Voltage fluctuation detected on transformer 2." }
                        };
                    }
                }

                if (alerts != null && alerts.Count > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"  {"AlertId",-10} {"FeederId",-12} {"Severity",-12} {"Broadcast Message"}");
                    Console.WriteLine("  " + new string('─', 86));
                    Console.ResetColor();

                    foreach (var a in alerts)
                    {
                        Console.Write($"  {a.AlertId,-10} {a.FeederId,-12} ");
                        if (a.Severity.Equals("High", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"{"HIGH",-12} ");
                        }
                        else if (a.Severity.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write($"{"MEDIUM",-12} ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($"{a.Severity.ToUpperInvariant(),-12} ");
                        }
                        Console.ResetColor();

                        Console.WriteLine(a.Message);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("  " + new string('─', 86));
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  [✓] Retrieved {alerts.Count} operational dispatch alert(s).");
                    Console.ResetColor();
                }

                PauseAndClear();
                break;

            case "0":
                running = false;
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  [✓] Operational session closed cleanly. Goodbye!");
                Console.ResetColor();
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  [✗] Invalid selection. Please choose an option between 0 and 5.");
                Console.ResetColor();
                PauseAndClear();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  [✗ ERROR] An unexpected exception occurred: {ex.Message}");
        Console.ResetColor();
        PauseAndClear();
    }
}

static void PrintHeader()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║              SMART METER RECONCILIATION SYSTEM               ║
║         Enterprise ADO.NET & REST API Synchronization        ║
╚══════════════════════════════════════════════════════════════╝");
    Console.ResetColor();
}

static void PauseAndClear()
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("  Press [Enter] to return to menu...");
    Console.ResetColor();
    Console.ReadLine();

    if (!Console.IsInputRedirected)
    {
        try { Console.Clear(); } catch { }
    }
}
