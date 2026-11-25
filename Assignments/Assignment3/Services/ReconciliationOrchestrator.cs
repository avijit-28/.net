using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment3.ApiClients;
using Assignment3.Models;
using Assignment3.Repositories;

namespace Assignment3.Services
{
    public class ReconciliationOrchestrator
    {
        private readonly MeterApiClient _apiClient;
        private readonly MeterReconciliationRepository _repository;

        public ReconciliationOrchestrator(
            MeterApiClient apiClient,
            MeterReconciliationRepository repository)
        {
            _apiClient = apiClient;
            _repository = repository;
        }

        public async Task<int> SyncMeterReadingsAsync(
            int? feederId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? cursor = null)
        {
            Guid runId = Guid.NewGuid();
            string batchId = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}";

            Console.WriteLine("\n[Reconciliation Run Initiated]");
            Console.WriteLine($"Run ID:   {runId}");
            Console.WriteLine($"Batch ID: {batchId}");
            Console.WriteLine("Fetching meter readings from API...");

            List<MeterReadingDto> readings;

            try
            {
                readings = await _apiClient.GetMeterReadingsAsync(
                    feederId,
                    startDate,
                    endDate,
                    cursor);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[API Unavailable / Connection Refused]");
                Console.WriteLine($"Details: {ex.Message}");
                Console.ResetColor();

                Console.Write("\nWould you like to simulate synchronization using test readings? (y/n): ");
                string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (input == "y" || input == "yes")
                {
                    readings = GetSampleTestReadings(batchId);
                    Console.WriteLine($"Loaded {readings.Count} test readings to verify end-to-end reconciliation.\n");
                }
                else
                {
                    // Log the failed sync run in database
                    await _repository.LogReconciliationRunAsync(new ReconciliationRunResult
                    {
                        RunId = runId,
                        BatchId = batchId,
                        TotalReadings = 0,
                        InsertedReadings = 0,
                        DuplicateReadings = 0,
                        RejectedReadings = 0,
                        AnomalyReadings = 0,
                        FailedReadings = 1,
                        Status = "Failed",
                        Message = $"API connection failed: {ex.Message}"
                    });

                    Console.WriteLine("Failed run logged to ReconciliationAuditLog.");
                    return 0;
                }
            }

            Console.WriteLine($"Processing {readings.Count} meter reading(s)...");

            int insertedCount = 0;
            int duplicateCount = 0;
            int rejectedCount = 0;
            int anomalyCount = 0;
            int failedCount = 0;

            foreach (MeterReadingDto reading in readings)
            {
                if (string.IsNullOrWhiteSpace(reading.BatchId))
                {
                    reading.BatchId = batchId;
                }

                try
                {
                    var (returnCode, status, message, readingId) =
                        await _repository.UpsertMeterReadingAsync(runId, reading);

                    if (returnCode == 1) // Inserted
                    {
                        insertedCount++;
                        Console.WriteLine($"  [INSERTED]   Meter {reading.MeterSerialNumber,-10} | Reading: {reading.CumulativeKWh,8:F3} kWh | {message}");

                        // Calculate consumption delta
                        if (readingId.HasValue)
                        {
                            var deltaResult = await _repository.CalculateConsumptionDeltaAsync(runId, readingId.Value);

                            if (deltaResult.Status == "Anomaly")
                            {
                                anomalyCount++;
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"               -> [ANOMALY] {deltaResult.Message}");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.WriteLine($"               -> [DELTA]   {deltaResult.Status}: {deltaResult.Message}");
                            }
                        }
                    }
                    else if (returnCode == 2) // Duplicate
                    {
                        duplicateCount++;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"  [DUPLICATE]  Meter {reading.MeterSerialNumber,-10} | {message}");
                        Console.ResetColor();
                    }
                    else if (returnCode < 0) // Rejected (e.g. unknown meter or negative reading)
                    {
                        rejectedCount++;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"  [REJECTED]   Meter {reading.MeterSerialNumber,-10} | {message}");
                        Console.ResetColor();
                    }
                    else
                    {
                        failedCount++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  [FAILED]     Meter {reading.MeterSerialNumber,-10} | {message}");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [ERROR]      Meter {reading.MeterSerialNumber,-10} | {ex.Message}");
                    Console.ResetColor();
                }
            }

            // Save run summary in database
            var runResult = new ReconciliationRunResult
            {
                RunId = runId,
                BatchId = batchId,
                TotalReadings = readings.Count,
                InsertedReadings = insertedCount,
                DuplicateReadings = duplicateCount,
                RejectedReadings = rejectedCount,
                AnomalyReadings = anomalyCount,
                FailedReadings = failedCount,
                Status = failedCount == 0 && rejectedCount == 0 ? "Success" : "CompletedWithAlerts",
                Message = $"Processed {readings.Count} readings: {insertedCount} inserted, {duplicateCount} duplicates, {anomalyCount} anomalies, {rejectedCount} rejected."
            };

            await _repository.LogReconciliationRunAsync(runResult);

            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("        SYNCHRONIZATION SUMMARY            ");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine($"Total Received:     {readings.Count}");
            Console.WriteLine($"Inserted:           {insertedCount}");
            Console.WriteLine($"Calculated Deltas:  {insertedCount}");
            Console.WriteLine($"Anomalies Detected: {anomalyCount}");
            Console.WriteLine($"Duplicates Skipped: {duplicateCount}");
            Console.WriteLine($"Rejected:           {rejectedCount}");
            Console.WriteLine($"Failed / Errors:    {failedCount}");
            Console.WriteLine("Summary saved to ReconciliationAuditLog.");
            Console.WriteLine("-------------------------------------------");

            return insertedCount;
        }

        private static List<MeterReadingDto> GetSampleTestReadings(string batchId)
        {
            DateTime now = DateTime.UtcNow;

            return new List<MeterReadingDto>
            {
                // Reading 1: Baseline / normal reading for MTR-1001
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-1001",
                    ExternalReadingId = $"EXT-{batchId}-001",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-5),
                    CumulativeKWh = 2000.000m
                },
                // Reading 2: Normal positive consumption for MTR-1001 (+25 kWh)
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-1001",
                    ExternalReadingId = $"EXT-{batchId}-002",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-3),
                    CumulativeKWh = 2025.000m
                },
                // Reading 3: Reading for MTR-3001
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-3001",
                    ExternalReadingId = $"EXT-{batchId}-003",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-4),
                    CumulativeKWh = 1500.000m
                },
                // Reading 4: LOWER reading for MTR-3001 (Negative Delta -> Triggers Anomaly!)
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-3001",
                    ExternalReadingId = $"EXT-{batchId}-004",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-2),
                    CumulativeKWh = 1420.000m
                },
                // Reading 5: Negative cumulative reading (Rejected by database)
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-2001",
                    ExternalReadingId = $"EXT-{batchId}-005",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-1),
                    CumulativeKWh = -50.000m
                },
                // Reading 6: Unknown meter serial number (Rejected by database)
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-UNKNOWN-999",
                    ExternalReadingId = $"EXT-{batchId}-006",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-1),
                    CumulativeKWh = 300.000m
                },
                // Reading 7: Duplicate of Reading 2 (Duplicate check)
                new MeterReadingDto
                {
                    MeterSerialNumber = "MTR-1001",
                    ExternalReadingId = $"EXT-{batchId}-002",
                    BatchId = batchId,
                    ReadingDate = now.AddMinutes(-3),
                    CumulativeKWh = 2025.000m
                }
            };
        }
    }
}
