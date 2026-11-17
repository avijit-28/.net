namespace HotelBooking.API.Controllers;

using System.Security.Claims;
using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> Create([FromBody] CreateBookingRequestDto request)
    {
        int userId = GetCurrentUserId();
        var created = await _bookingService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, ApiResponse<BookingResponseDto>.Ok(created, "Booking confirmed successfully."));
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<BookingResponseDto>>>> GetMyBookings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        int userId = GetCurrentUserId();
        var list = await _bookingService.GetByUserAsync(userId, pageNumber, pageSize);
        return Ok(ApiResponse<List<BookingResponseDto>>.Ok(list));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> GetById(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null)
            return NotFound(ApiResponse<BookingDetailDto>.Fail("Booking not found."));

        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && booking.UserId != currentUserId)
        {
            return Forbid();
        }

        return Ok(ApiResponse<BookingDetailDto>.Ok(booking));
    }

    [HttpGet("ref/{reference}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> GetByReference(string reference)
    {
        var booking = await _bookingService.GetByReferenceAsync(reference);
        if (booking == null)
            return NotFound(ApiResponse<BookingDetailDto>.Fail("Booking not found."));

        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && booking.UserId != currentUserId)
        {
            return Forbid();
        }

        return Ok(ApiResponse<BookingDetailDto>.Ok(booking));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<BookingDetailDto>>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _bookingService.GetAllAsync(status, startDate, endDate, pageNumber, pageSize);
        return Ok(ApiResponse<PagedResultDto<BookingDetailDto>>.Ok(result));
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
