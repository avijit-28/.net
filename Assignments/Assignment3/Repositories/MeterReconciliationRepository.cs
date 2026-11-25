using System;
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

        public async Task TestConnectionAsync()
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync();

            Console.WriteLine("SQL Server connection successful.");
        }


        public async Task<(int ReturnCode, string Status, string Message)>
            UpsertMeterReadingAsync(MeterReadingDto reading)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand("dbo.usp_UpsertMeterReading", connection);

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

            statusParameter.Direction = ParameterDirection.Output;

            SqlParameter messageParameter =
                command.Parameters.Add(
                    "@Message",
                    SqlDbType.NVarChar,
                    500);

            messageParameter.Direction = ParameterDirection.Output;

            SqlParameter returnParameter =
                command.Parameters.Add(
                    "@RETURN_VALUE",
                    SqlDbType.Int);

            returnParameter.Direction = ParameterDirection.ReturnValue;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            int returnCode =
                (int)returnParameter.Value;

            string status =
                statusParameter.Value?.ToString() ?? string.Empty;

            string message =
                messageParameter.Value?.ToString() ?? string.Empty;

            return (returnCode, status, message);
        }


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

            command.CommandType = CommandType.StoredProcedure;

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


    }
}
