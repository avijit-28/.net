namespace HotelBooking.API.Controllers;

using System.Security.Claims;
using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Handles atomic room reservations, concurrency safety locks, and booking audits.
/// </summary>
[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Creates an atomic room reservation with UPDLOCK/ROWLOCK and UPDLOCK/HOLDLOCK concurrency protection.
    /// </summary>
    /// <param name="request">Booking specifications (roomId, checkInDate, checkOutDate, numberOfGuests).</param>
    /// <returns>Confirmed booking response with generated BookingReference (BK-YYYYMMDD-XXXX).</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> Create([FromBody] CreateBookingRequestDto request)
    {
        int userId = GetCurrentUserId();
        var created = await _bookingService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, ApiResponse<BookingResponseDto>.Ok(created, "Booking confirmed successfully."));
    }

    /// <summary>
    /// Retrieves paginated booking history for the currently authenticated user.
    /// </summary>
    /// <param name="pageNumber">Page index (default 1).</param>
    /// <param name="pageSize">Number of items per page (default 10).</param>
    /// <returns>List of user bookings.</returns>
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

    /// <summary>
    /// Retrieves detailed booking information by ID. Restricted to booking owner or Administrators.
    /// </summary>
    /// <param name="id">Booking identifier.</param>
    /// <returns>Booking details including user and room metadata.</returns>
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

    /// <summary>
    /// Retrieves booking details using the unique public Booking Reference code.
    /// </summary>
    /// <param name="reference">Human-readable booking reference (e.g., BK-20261113-0001).</param>
    /// <returns>Booking details.</returns>
    [HttpGet("ref/{reference}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> GetByReference(string reference)
    {
        var booking = await _bookingService.GetByReferenceAsync(reference);
        if (booking == null)
            return NotFound(ApiResponse<BookingDetailDto>.Fail("Booking reference not found."));

        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && booking.UserId != currentUserId)
        {
            return Forbid();
        }

        return Ok(ApiResponse<BookingDetailDto>.Ok(booking));
    }

    /// <summary>
    /// Global booking audit listing with date and status filters. Restricted to Administrators.
    /// </summary>
    /// <param name="status">Optional status filter (Confirmed, Cancelled, Completed).</param>
    /// <param name="startDate">Optional start date range.</param>
    /// <param name="endDate">Optional end date range.</param>
    /// <param name="pageNumber">Page index (default 1).</param>
    /// <param name="pageSize">Page size (default 20).</param>
    /// <returns>Paginated booking records.</returns>
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
        return int.TryParse(claim, out int id) ? id : 0;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
