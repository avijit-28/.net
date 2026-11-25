
using System;
using System.Net.Http;
using Assignment3.Configuration;

namespace Assignment3.ApiClients
{
    public class MeterApiClient
    {
        private readonly HttpClient _httpClient;

        public MeterApiClient(AppSettings settings)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(settings.ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(
                    settings.TimeoutSeconds)
            };

            if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add(
                    "X-API-Key",
                    settings.ApiKey);
            }
        }
    }
}
