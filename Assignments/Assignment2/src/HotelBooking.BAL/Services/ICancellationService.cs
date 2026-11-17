namespace HotelBooking.BAL.Services;

using HotelBooking.Models.DTOs;

public interface ICancellationService
{
    Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason);
    Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId);
}
