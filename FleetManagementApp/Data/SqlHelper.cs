using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FleetManagementApp.Data
{
    /// <summary>
    /// Centralized asynchronous ADO.NET helper compatible with .NET Framework (C# 7.3+).
    /// Uses deterministic 'using' disposal to eliminate connection and resource leaks.
    /// </summary>
    public class SqlHelper
    {
        private readonly string _connectionString;

        public SqlHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["FleetDbConnection"]?.ConnectionString
                ?? throw new InvalidOperationException("Connection string 'FleetDbConnection' not found in App.config.");
        }

        public SqlHelper(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Executes an INSERT, UPDATE, or DELETE query and returns the number of affected rows.
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Executes a query that returns a single scalar value (e.g., SCOPE_IDENTITY or COUNT).
        /// </summary>
        public async Task<T> ExecuteScalarAsync<T>(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                    if (result == null || result == DBNull.Value)
                    {
                        return default(T);
                    }

                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }

        /// <summary>
        /// Executes a SELECT query and maps multiple rows into a List.
        /// </summary>
        public async Task<List<T>> ExecuteReaderAsync<T>(string sql, Func<SqlDataReader, T> rowMapper, params SqlParameter[] parameters)
        {
            var results = new List<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            results.Add(rowMapper(reader));
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Executes a SELECT query and maps a single record (or default if not found).
        /// </summary>
        public async Task<T> ExecuteSingleAsync<T>(string sql, Func<SqlDataReader, T> rowMapper, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            return rowMapper(reader);
                        }
                    }
                }
            }

            return default(T);
        }
    }
}