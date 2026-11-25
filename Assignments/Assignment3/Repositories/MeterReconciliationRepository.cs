
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Assignment3.Configuration;
using Assignment3.Models;

namespace Assignment3.Repositories
{
    public class MeterReconciliationRepository
    {
        private readonly string _connectionString;

        public MeterReconciliationRepository(AppSettings settings)
        {
            _connectionString = settings.ConnectionString;
        }

        // 1. Test SQL Server connection
        public async Task TestConnectionAsync()
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync();

            Console.WriteLine("SQL Server connection successful.");
        }

        // 2. Insert a meter reading
        public async Task<(int ReturnCode, string Status, string Message)>
            UpsertMeterReadingAsync(MeterReadingDto reading)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "dbo.usp_UpsertMeterReading",
                    connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@MeterSerialNumber",
                SqlDbType.NVarChar,
                50).Value = reading.MeterSerialNumber;

            command.Parameters.Add(
                "@ExternalReadingId",
                SqlDbType.NVarChar,
                100).Value =
                    (object?)reading.ExternalReadingId ?? DBNull.Value;

            command.Parameters.Add(
                "@BatchId",
                SqlDbType.NVarChar,
                100).Value = reading.BatchId;

            command.Parameters.Add(
                "@ReadingDate",
                SqlDbType.DateTime2).Value = reading.ReadingDate;

            SqlParameter cumulativeParameter =
                command.Parameters.Add(
                    "@CumulativeKWh",
                    SqlDbType.Decimal);

            cumulativeParameter.Precision = 18;
            cumulativeParameter.Scale = 3;
            cumulativeParameter.Value = reading.CumulativeKWh;

            SqlParameter statusParameter =
                command.Parameters.Add(
                    "@Status",
                    SqlDbType.NVarChar,
                    20);

            statusParameter.Direction =
                ParameterDirection.Output;

            SqlParameter messageParameter =
                command.Parameters.Add(
                    "@Message",
                    SqlDbType.NVarChar,
                    500);

            messageParameter.Direction =
                ParameterDirection.Output;

            SqlParameter returnParameter =
                command.Parameters.Add(
                    "@RETURN_VALUE",
                    SqlDbType.Int);

            returnParameter.Direction =
                ParameterDirection.ReturnValue;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            int returnCode =
                (int)returnParameter.Value;

            string status =
                statusParameter.Value?.ToString()
                ?? string.Empty;

            string message =
                messageParameter.Value?.ToString()
                ?? string.Empty;

            return (returnCode, status, message);
        }

        // 3. Calculate consumption delta
        public async Task<int> CalculateConsumptionDeltaAsync(
            Guid runId,
            long readingId)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "dbo.usp_CalculateConsumptionDelta",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@RunId",
                SqlDbType.UniqueIdentifier).Value = runId;

            command.Parameters.Add(
                "@ReadingId",
                SqlDbType.BigInt).Value = readingId;

            SqlParameter returnParameter =
                command.Parameters.Add(
                    "@RETURN_VALUE",
                    SqlDbType.Int);

            returnParameter.Direction =
                ParameterDirection.ReturnValue;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return (int)returnParameter.Value;
        }

        // 4. Retrieve anomalous readings
        public async Task<List<AnomalousReadingRecord>>
            GetAnomalousReadingsAsync(
                int? feederId = null,
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            List<AnomalousReadingRecord> readings = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "dbo.usp_GetAnomalousReadings",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@FeederId",
                SqlDbType.Int).Value =
                    (object?)feederId ?? DBNull.Value;

            command.Parameters.Add(
                "@StartDate",
                SqlDbType.Date).Value =
                    (object?)startDate ?? DBNull.Value;

            command.Parameters.Add(
                "@EndDate",
                SqlDbType.Date).Value =
                    (object?)endDate ?? DBNull.Value;

            await connection.OpenAsync();

            using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                readings.Add(new AnomalousReadingRecord
                {
                    ReadingId = reader.GetInt64(
                        reader.GetOrdinal("ReadingId")),

                    MeterSerialNumber = reader.GetString(
                        reader.GetOrdinal("MeterSerialNumber")),

                    FeederId = reader.IsDBNull(
                        reader.GetOrdinal("FeederId"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("FeederId")),

                    ReadingDate = reader.GetDateTime(
                        reader.GetOrdinal("ReadingDate")),

                    CumulativeKWh = reader.GetDecimal(
                        reader.GetOrdinal("CumulativeKWh")),

                    PreviousCumulativeKWh = reader.IsDBNull(
                        reader.GetOrdinal("PreviousCumulativeKWh"))
                        ? null
                        : reader.GetDecimal(
                            reader.GetOrdinal("PreviousCumulativeKWh")),

                    ConsumptionKWh = reader.IsDBNull(
                        reader.GetOrdinal("ConsumptionKWh"))
                        ? null
                        : reader.GetDecimal(
                            reader.GetOrdinal("ConsumptionKWh")),

                    ReadingStatus = reader.GetString(
                        reader.GetOrdinal("ReadingStatus"))
                });
            }

            return readings;
        }

        // 5. Log reconciliation run
        public async Task<(int ReturnCode, string Status, string Message)>
            LogReconciliationRunAsync(
                ReconciliationRunResult result)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "dbo.usp_LogReconciliationRun",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@RunId",
                SqlDbType.UniqueIdentifier).Value = result.RunId;

            command.Parameters.Add(
                "@BatchId",
                SqlDbType.NVarChar,
                100).Value = result.BatchId;

            command.Parameters.Add(
                "@TotalReadings",
                SqlDbType.Int).Value = result.TotalReadings;

            command.Parameters.Add(
                "@InsertedReadings",
                SqlDbType.Int).Value = result.InsertedReadings;

            command.Parameters.Add(
                "@DuplicateReadings",
                SqlDbType.Int).Value = result.DuplicateReadings;

            command.Parameters.Add(
                "@RejectedReadings",
                SqlDbType.Int).Value = result.RejectedReadings;

            command.Parameters.Add(
                "@AnomalyReadings",
                SqlDbType.Int).Value = result.AnomalyReadings;

            command.Parameters.Add(
                "@FailedReadings",
                SqlDbType.Int).Value = result.FailedReadings;

            SqlParameter statusParameter =
                command.Parameters.Add(
                    "@Status",
                    SqlDbType.NVarChar,
                    20);

            statusParameter.Direction =
                ParameterDirection.Output;

            SqlParameter messageParameter =
                command.Parameters.Add(
                    "@Message",
                    SqlDbType.NVarChar,
                    500);

            messageParameter.Direction =
                ParameterDirection.Output;

            SqlParameter returnParameter =
                command.Parameters.Add(
                    "@RETURN_VALUE",
                    SqlDbType.Int);

            returnParameter.Direction =
                ParameterDirection.ReturnValue;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            int returnCode =
                (int)returnParameter.Value;

            string status =
                statusParameter.Value?.ToString()
                ?? string.Empty;

            string message =
                messageParameter.Value?.ToString()
                ?? string.Empty;

            return (returnCode, status, message);
        }

        // 6. Retrieve reconciliation summaries
        public async Task<DataTable> GetReconciliationSummaryAsync(
            Guid? runId = null,
            string? batchId = null)
        {
            DataTable summaryTable = new DataTable();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "dbo.usp_GetReconciliationSummary",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.Add(
                "@RunId",
                SqlDbType.UniqueIdentifier).Value =
                    (object?)runId ?? DBNull.Value;

            command.Parameters.Add(
                "@BatchId",
                SqlDbType.NVarChar,
                100).Value =
                    (object?)batchId ?? DBNull.Value;

            await connection.OpenAsync();

            using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            summaryTable.Load(reader);

            return summaryTable;
        }
    }
}
