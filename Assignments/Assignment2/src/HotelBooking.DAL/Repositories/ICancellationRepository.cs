namespace HotelBooking.DAL.Repositories;

using HotelBooking.Models.DTOs;

public interface ICancellationRepository
{
    Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason);
    Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId);
}
