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
            ReadingDate = now.AddMinutes(-12),
            CumulativeKWh = 1250.000m
        },
        // 2. Valid Subsequent Reading for MTR-1001 (+35.5 kWh consumption)
        new()
        {
            MeterSerialNumber = "MTR-1001",
            ExternalReadingId = $"EXT-{batchId}-002",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-10),
            CumulativeKWh = 1285.500m
        },
        // 3. Normal Baseline Reading for MTR-1002
        new()
        {
            MeterSerialNumber = "MTR-1002",
            ExternalReadingId = $"EXT-{batchId}-003",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-11),
            CumulativeKWh = 900.000m
        },
        // 4. Normal Subsequent Reading for MTR-1002 (+32.1 kWh consumption)
        new()
        {
            MeterSerialNumber = "MTR-1002",
            ExternalReadingId = $"EXT-{batchId}-004",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-6),
            CumulativeKWh = 932.100m
        },
        // 5. Valid Baseline Reading for MTR-2001
        new()
        {
            MeterSerialNumber = "MTR-2001",
            ExternalReadingId = $"EXT-{batchId}-005",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-10),
            CumulativeKWh = 1450.000m
        },
        // 6. Valid Subsequent Reading for MTR-2001 (+38.25 kWh consumption)
        new()
        {
            MeterSerialNumber = "MTR-2001",
            ExternalReadingId = $"EXT-{batchId}-006",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-5),
            CumulativeKWh = 1488.250m
        },
        // 7. Valid Baseline Reading for MTR-2002
        new()
        {
            MeterSerialNumber = "MTR-2002",
            ExternalReadingId = $"EXT-{batchId}-007",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-9),
            CumulativeKWh = 780.000m
        },
        // 8. Valid Subsequent Reading for MTR-2002 (+32.6 kWh consumption)
        new()
        {
            MeterSerialNumber = "MTR-2002",
            ExternalReadingId = $"EXT-{batchId}-008",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-4),
            CumulativeKWh = 812.600m
        },
        // 9. Anomaly for MTR-3001 (High reading followed by lower reading)
        new()
        {
            MeterSerialNumber = "MTR-3001",
            ExternalReadingId = $"EXT-{batchId}-009",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-7),
            CumulativeKWh = 2100.000m
        },
        new()
        {
            MeterSerialNumber = "MTR-3001",
            ExternalReadingId = $"EXT-{batchId}-010",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-2),
            CumulativeKWh = 2050.000m // Negative delta -> Anomaly!
        },
        // 11. Invalid negative reading -> Rejected
        new()
        {
            MeterSerialNumber = "MTR-2001",
            ExternalReadingId = $"EXT-{batchId}-011",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-1),
            CumulativeKWh = -25.000m
        },
        // 12. Unknown Meter -> Rejected
        new()
        {
            MeterSerialNumber = "MTR-UNKNOWN-888",
            ExternalReadingId = $"EXT-{batchId}-012",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-1),
            CumulativeKWh = 400.000m
        },
        // 13. Duplicate of Reading 2
        new()
        {
            MeterSerialNumber = "MTR-1001",
            ExternalReadingId = $"EXT-{batchId}-002",
            BatchId = batchId,
            ReadingDate = now.AddMinutes(-10),
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
        new { SlabId = 3, FeederId = "FDR-02", SlabName = "Commercial Tier 1", MinUnits = 0m, MaxUnits = (decimal?)500m, RatePerUnit = 8.50m },
        new { SlabId = 4, FeederId = "FDR-02", SlabName = "Commercial Tier 2", MinUnits = 501m, MaxUnits = (decimal?)2000m, RatePerUnit = 10.75m },
        new { SlabId = 5, FeederId = "FDR-03", SlabName = "Industrial High-Tension", MinUnits = 0m, MaxUnits = (decimal?)null, RatePerUnit = 12.50m },
        new { SlabId = 6, FeederId = "FDR-01", SlabName = "Agricultural Concessional", MinUnits = 0m, MaxUnits = (decimal?)1000m, RatePerUnit = 2.80m }
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
        new { AlertId = "ALT-002", FeederId = "FDR-03", Severity = "High", Message = "Voltage fluctuation detected on transformer 2." },
        new { AlertId = "ALT-003", FeederId = "FDR-02", Severity = "Medium", Message = "Feeder load crossed 85% operating capacity during peak hours." },
        new { AlertId = "ALT-004", FeederId = "FDR-01", Severity = "Critical", Message = "Emergency breaker tripped: Overcurrent protection activated." },
        new { AlertId = "ALT-005", FeederId = "FDR-03", Severity = "Low", Message = "Scheduled routine transformer oil filtration." }
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
