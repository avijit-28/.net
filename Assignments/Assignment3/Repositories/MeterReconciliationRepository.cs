using System;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Assignment3.Configuration;

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
    }
}
