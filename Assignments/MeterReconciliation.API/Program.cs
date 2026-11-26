using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 1. Meter Readings Batch API
app.MapGet("/api/v1/meter-readings/batch", (int? feederId, DateTime? startDate, DateTime? endDate, string? cursor) =>
{
    string batchId = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}";
    DateTime now = DateTime.UtcNow;

    var readings = new List<MeterReadingResponse>
    {
        // 1. Valid Baseline Reading for MTR-1001
        new()
        {
            MeterSerialNumber = "MTR-1001",
            ExternalReadingId = $"EXT-{batchId}-001",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-10),
            CumulativeKWh = 1250.000m
        },
        // 2. Valid Subsequent Reading for MTR-1001 (+35.5 kWh consumption)
        new()
        {
            MeterSerialNumber = "MTR-1001",
            ExternalReadingId = $"EXT-{batchId}-002",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-5),
            CumulativeKWh = 1285.500m
        },
        // 3. Normal Reading for MTR-1002
        new()
        {
            MeterSerialNumber = "MTR-1002",
            ExternalReadingId = $"EXT-{batchId}-003",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-8),
            CumulativeKWh = 900.000m
        },
        // 4. Anomaly for MTR-3001 (High reading followed by lower reading)
        new()
        {
            MeterSerialNumber = "MTR-3001",
            ExternalReadingId = $"EXT-{batchId}-004",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-6),
            CumulativeKWh = 2100.000m
        },
        new()
        {
            MeterSerialNumber = "MTR-3001",
            ExternalReadingId = $"EXT-{batchId}-005",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-2),
            CumulativeKWh = 2050.000m // Negative delta -> Anomaly!
        },
        // 5. Invalid negative reading -> Rejected
        new()
        {
            MeterSerialNumber = "MTR-2001",
            ExternalReadingId = $"EXT-{batchId}-006",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-1),
            CumulativeKWh = -10.000m
        },
        // 6. Unknown Meter -> Rejected
        new()
        {
            MeterSerialNumber = "MTR-UNKNOWN-888",
            ExternalReadingId = $"EXT-{batchId}-007",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-1),
            CumulativeKWh = 400.000m
        },
        // 7. Duplicate of Reading 2
        new()
        {
            MeterSerialNumber = "MTR-1001",
            ExternalReadingId = $"EXT-{batchId}-002",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-5),
            CumulativeKWh = 1285.500m
        }
    };

    return Results.Ok(readings);
})
.WithName("GetMeterReadingsBatch");

// 2. Tariff Updates API
app.MapGet("/api/v1/tariff-updates", (string? feederId) =>
{
    var tariffs = new[]
    {
        new { SlabId = 1, FeederId = "FDR-01", SlabName = "Domestic Tier 1", MinUnits = 0m, MaxUnits = (decimal?)100m, RatePerUnit = 4.50m },
        new { SlabId = 2, FeederId = "FDR-01", SlabName = "Domestic Tier 2", MinUnits = 101m, MaxUnits = (decimal?)300m, RatePerUnit = 6.25m },
        new { SlabId = 3, FeederId = "FDR-02", SlabName = "Commercial Tier 1", MinUnits = 0m, MaxUnits = (decimal?)500m, RatePerUnit = 8.50m }
    };

    return Results.Ok(tariffs);
})
.WithName("GetTariffUpdates");

// 3. Outage & Dispatch Alerts API
app.MapGet("/api/v1/dispatch-alerts", (string? feederId) =>
{
    var alerts = new[]
    {
        new { AlertId = "ALT-001", FeederId = "FDR-01", Severity = "Low", Message = "Scheduled maintenance at Substation Alpha." },
        new { AlertId = "ALT-002", FeederId = "FDR-03", Severity = "High", Message = "Voltage fluctuation detected on transformer 2." }
    };

    return Results.Ok(alerts);
})
.WithName("GetDispatchAlerts");

app.Run();

public class MeterReadingResponse
{
    [JsonPropertyName("meterSerialNumber")]
    public string MeterSerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("externalReadingId")]
    public string? ExternalReadingId { get; set; }

    [JsonPropertyName("batchId")]
    public string BatchId { get; set; } = string.Empty;

    [JsonPropertyName("readingDate")]
    public DateTime ReadingDate { get; set; }

    [JsonPropertyName("cumulativeKWh")]
    public decimal CumulativeKWh { get; set; }
}
