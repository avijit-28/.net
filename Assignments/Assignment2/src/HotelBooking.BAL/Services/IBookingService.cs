namespace HotelBooking.BAL.Services;

using HotelBooking.Models.DTOs;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(int userId, CreateBookingRequestDto dto);
    Task<BookingDetailDto?> GetByIdAsync(int bookingId);
    Task<BookingDetailDto?> GetByReferenceAsync(string bookingReference);
    Task<List<BookingResponseDto>> GetByUserAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<PagedResultDto<BookingDetailDto>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 20);
}
