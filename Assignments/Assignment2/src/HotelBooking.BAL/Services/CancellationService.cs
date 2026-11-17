namespace HotelBooking.BAL.Services;

using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;

public class CancellationService : ICancellationService
{
    private readonly ICancellationRepository _repository;

    public CancellationService(ICancellationRepository repository)
    {
        _repository = repository;
    }

    public Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason) => _repository.CancelBookingAsync(bookingId, cancelledByUserId, reason);
    public Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId) => _repository.GetDetailsByBookingIdAsync(bookingId);
}
