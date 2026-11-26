
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Assignment3.Configuration;
using Assignment3.Models;

namespace Assignment3.ApiClients
{
    public class MeterApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly int _maxRetryAttempts;

        public MeterApiClient(AppSettings settings)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(settings.ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(
                    settings.TimeoutSeconds)
            };

            _maxRetryAttempts = settings.MaxRetryAttempts;

            if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add(
                    "X-API-Key",
                    settings.ApiKey);
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"));
        }


        public async Task<List<MeterReadingDto>>
            GetMeterReadingsAsync(
                int? feederId = null,
                DateTime? startDate = null,
                DateTime? endDate = null,
                string? cursor = null)
        {
            string endpoint =
                "api/v1/meter-readings/batch";

            List<string> queryParameters = new();

            if (feederId.HasValue)
            {
                queryParameters.Add(
                    $"feederId={feederId.Value}");
            }

            if (startDate.HasValue)
            {
                queryParameters.Add(
                    $"startDate={Uri.EscapeDataString(
                        startDate.Value.ToString(
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture))}");
            }

            if (endDate.HasValue)
            {
                queryParameters.Add(
                    $"endDate={Uri.EscapeDataString(
                        endDate.Value.ToString(
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture))}");
            }

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                queryParameters.Add(
                    $"cursor={Uri.EscapeDataString(cursor)}");
            }

            if (queryParameters.Count > 0)
            {
                endpoint += "?" +
                    string.Join("&", queryParameters);
            }

            Exception? lastException = null;

            for (int attempt = 1;
                 attempt <= _maxRetryAttempts;
                 attempt++)
            {
                try
                {
                    using HttpResponseMessage response =
                        await _httpClient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string json =
                            await response.Content.ReadAsStringAsync();
                            

                        List<MeterReadingDto>? readings =
                            JsonSerializer.Deserialize<
                                List<MeterReadingDto>>(
                                    json,
                                    new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    });

                        return readings
                            ?? new List<MeterReadingDto>();
                    }

                    int statusCode =
                        (int)response.StatusCode;

                    if (statusCode == 400 ||
                        statusCode == 401 ||
                        statusCode == 404)
                    {
                        throw new HttpRequestException(
                            $"API request failed. HTTP status: {statusCode} ({response.ReasonPhrase})");
                    }

                    if (statusCode != 429 &&
                        statusCode != 503)
                    {
                        throw new HttpRequestException(
                            $"API request failed. HTTP status: {statusCode} ({response.ReasonPhrase})");
                    }

                    if (attempt == _maxRetryAttempts)
                    {
                        throw new HttpRequestException(
                            $"API request failed after {attempt} attempts. " +
                            $"HTTP status: {statusCode} ({response.ReasonPhrase})");
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    if (attempt < _maxRetryAttempts)
                    {
                        Console.WriteLine($"[API Retry {attempt}/{_maxRetryAttempts}] Connection failed: {ex.Message}. Retrying...");
                    }
                }
                catch (TaskCanceledException ex)
                {
                    lastException = ex;
                    if (attempt < _maxRetryAttempts)
                    {
                        Console.WriteLine($"[API Retry {attempt}/{_maxRetryAttempts}] Request timed out. Retrying...");
                    }
                }

                if (attempt < _maxRetryAttempts)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(attempt * 2));
                }
            }

            throw new HttpRequestException(
                $"Unable to connect to Web API at {_httpClient.BaseAddress} after {_maxRetryAttempts} attempts. " +
                $"Please ensure the API server is running and accessible. (Error: {lastException?.Message})",
                lastException);
        }

        public async Task<List<TariffUpdateDto>> GetTariffUpdatesAsync(string? feederId = null)
        {
            string endpoint = "api/v1/tariff-updates";
            if (!string.IsNullOrWhiteSpace(feederId))
            {
                endpoint += $"?feederId={Uri.EscapeDataString(feederId)}";
            }

            Exception? lastException = null;

            for (int attempt = 1; attempt <= _maxRetryAttempts; attempt++)
            {
                try
                {
                    using HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var tariffs = JsonSerializer.Deserialize<List<TariffUpdateDto>>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        return tariffs ?? new List<TariffUpdateDto>();
                    }

                    int statusCode = (int)response.StatusCode;
                    if (statusCode != 429 && statusCode != 503)
                    {
                        throw new HttpRequestException($"API request failed. HTTP status: {statusCode} ({response.ReasonPhrase})");
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    if (attempt < _maxRetryAttempts)
                    {
                        Console.WriteLine($"[API Retry {attempt}/{_maxRetryAttempts}] Connection failed: {ex.Message}. Retrying...");
                    }
                }
                catch (TaskCanceledException ex)
                {
                    lastException = ex;
                    if (attempt < _maxRetryAttempts)
                    {
                        Console.WriteLine($"[API Retry {attempt}/{_maxRetryAttempts}] Request timed out. Retrying...");
                    }
                }

                if (attempt < _maxRetryAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
                }
            }

            throw new HttpRequestException(
                $"Unable to connect to Web API at {_httpClient.BaseAddress} after {_maxRetryAttempts} attempts. " +
                $"Please ensure the API server is running and accessible. (Error: {lastException?.Message})",
                lastException);
        }

        public async Task<List<DispatchAlertDto>> GetDispatchAlertsAsync(string? feederId = null)
        {
            string endpoint = "api/v1/dispatch-alerts";
            if (!string.IsNullOrWhiteSpace(feederId))
            {
                endpoint += $"?feederId={Uri.EscapeDataString(feederId)}";
            }

            Exception? lastException = null;

            for (int attempt = 1; attempt <= _maxRetryAttempts; attempt++)
            {
                try
                {
                    using HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var alerts = JsonSerializer.Deserialize<List<DispatchAlertDto>>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        return alerts ?? new List<DispatchAlertDto>();
                    }

                    int statusCode = (int)response.StatusCode;
                    if (statusCode != 429 && statusCode != 503)
                    {
                        throw new HttpRequestException($"API request failed. HTTP status: {statusCode} ({response.ReasonPhrase})");
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    if (attempt < _maxRetryAttempts)
                    {
                        Console.WriteLine($"[API Retry {attempt}/{_maxRetryAttempts}] Connection failed: {ex.Message}. Retrying...");
                    }
                }
                catch (TaskCanceledException ex)
                {
                    lastException = ex;
                    if (attempt < _maxRetryAttempts)
                    {
                        Console.WriteLine($"[API Retry {attempt}/{_maxRetryAttempts}] Request timed out. Retrying...");
                    }
                }

                if (attempt < _maxRetryAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
                }
            }

            throw new HttpRequestException(
                $"Unable to connect to Web API at {_httpClient.BaseAddress} after {_maxRetryAttempts} attempts. " +
                $"Please ensure the API server is running and accessible. (Error: {lastException?.Message})",
                lastException);
        }
    }
}
