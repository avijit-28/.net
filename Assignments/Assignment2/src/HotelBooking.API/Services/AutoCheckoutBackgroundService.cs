namespace HotelBooking.API.Services;

using HotelBooking.BAL.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background worker that periodically checks for bookings whose check-out dates have passed
/// and automatically checks them out, updating room statuses back to Available.
/// </summary>
public class AutoCheckoutBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutoCheckoutBackgroundService> _logger;

    public AutoCheckoutBackgroundService(IServiceProvider serviceProvider, ILogger<AutoCheckoutBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AutoCheckout Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                await bookingService.AutoCheckoutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during automated background room checkout.");
            }

            // Run check every 1 minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
