
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
            Console.WriteLine("Fetching meter readings from API...");

            List<MeterReadingDto> readings =
                await _apiClient.GetMeterReadingsAsync(
                    feederId,
                    startDate,
                    endDate,
                    cursor);

            Console.WriteLine(
                $"Received {readings.Count} meter readings.");

            int successfulRecords = 0;
            int failedRecords = 0;

            foreach (MeterReadingDto reading in readings)
            {
                try
                {
                    var result =
                        await _repository.UpsertMeterReadingAsync(reading);

                    if (result.ReturnCode == 0)
                    {
                        successfulRecords++;

                        Console.WriteLine(
                            $"Saved: {result.Status}");
                    }
                    else
                    {
                        failedRecords++;

                        Console.WriteLine(
                            $"Failed: {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    failedRecords++;

                    Console.WriteLine(
                        $"Record processing error: {ex.Message}");
                }
            }

            Console.WriteLine("\nSynchronization completed.");
            Console.WriteLine($"Successful records: {successfulRecords}");
            Console.WriteLine($"Failed records: {failedRecords}");

            return successfulRecords;
        }
    }
}
