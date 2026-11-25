using System;
using Microsoft.Extensions.Configuration;

namespace Assignment3.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; private set; } = string.Empty;

        public string ApiBaseUrl { get; private set; } = string.Empty;

        public string ApiKey { get; private set; } = string.Empty;

        public int TimeoutSeconds { get; private set; }

        public int MaxRetryAttempts { get; private set; }

        public static AppSettings Load(IConfiguration configuration)
        {
            return new AppSettings
            {
                ConnectionString =
                    configuration.GetConnectionString("MeterRecon")
                    ?? throw new InvalidOperationException(
                        "MeterRecon connection string is missing."),

                ApiBaseUrl =
                    configuration["ApiSettings:BaseUrl"]
                    ?? throw new InvalidOperationException(
                        "API BaseUrl is missing."),

                ApiKey =
                    configuration["ApiSettings:ApiKey"] ?? string.Empty,

                TimeoutSeconds =
                    configuration.GetValue<int>(
                        "ApiSettings:TimeoutSeconds"),

                MaxRetryAttempts =
                    configuration.GetValue<int>(
                        "ApiSettings:MaxRetryAttempts")
            };
        }
    }
}
