namespace HotelBooking.BAL.Services;

using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public Task<BookingResponseDto> CreateAsync(int userId, CreateBookingRequestDto dto) => _repository.CreateAsync(userId, dto);
    public Task<BookingDetailDto?> GetByIdAsync(int bookingId) => _repository.GetByIdAsync(bookingId);
    public Task<BookingDetailDto?> GetByReferenceAsync(string bookingReference) => _repository.GetByReferenceAsync(bookingReference);
    public Task<List<BookingResponseDto>> GetByUserAsync(int userId, int pageNumber = 1, int pageSize = 10) => _repository.GetByUserAsync(userId, pageNumber, pageSize);
    public Task<PagedResultDto<BookingDetailDto>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 20) => _repository.GetAllAsync(status, startDate, endDate, pageNumber, pageSize);
}
