namespace HotelBooking.API.Controllers;

using System.Security.Claims;
using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Manages booking cancellations, automated 3-tier refund calculations, and audit records.
/// </summary>
[ApiController]
public class CancellationsController : ControllerBase
{
    private readonly ICancellationService _cancellationService;
    private readonly IBookingService _bookingService;

    public CancellationsController(ICancellationService cancellationService, IBookingService bookingService)
    {
        _cancellationService = cancellationService;
        _bookingService = bookingService;
    }

    /// <summary>
    /// Cancels an existing booking and calculates refund breakdown according to 3-tier refund policy.
    /// Tier 1 (>48h before check-in): 100% refund.
    /// Tier 2 (24-48h before check-in): 50% refund.
    /// Tier 3 (&lt;24h before check-in): 0% refund.
    /// </summary>
    /// <param name="bookingId">Booking identifier to cancel.</param>
    /// <param name="request">Optional cancellation reason.</param>
    /// <returns>Cancellation and financial refund breakdown.</returns>
    [HttpPost("api/bookings/{bookingId:int}/cancel")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CancellationResponseDto>>> CancelBooking(int bookingId, [FromBody] CancelBookingRequestDto request)
    {
        var booking = await _bookingService.GetByIdAsync(bookingId);
        if (booking == null)
            return NotFound(ApiResponse<CancellationResponseDto>.Fail("Booking not found."));

        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && booking.UserId != currentUserId)
        {
            return Forbid();
        }

        var result = await _cancellationService.CancelBookingAsync(bookingId, currentUserId, request.Reason);
        return Ok(ApiResponse<CancellationResponseDto>.Ok(result, "Booking cancelled successfully."));
    }

    /// <summary>
    /// Retrieves financial refund and charge breakdown details for a cancelled booking.
    /// </summary>
    /// <param name="bookingId">Booking identifier.</param>
    /// <returns>Cancellation details including refund percentage and charge amount.</returns>
    [HttpGet("api/cancellations/{bookingId:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CancellationResponseDto>>> GetDetails(int bookingId)
    {
        var details = await _cancellationService.GetDetailsByBookingIdAsync(bookingId);
        if (details == null)
            return NotFound(ApiResponse<CancellationResponseDto>.Fail("Cancellation record not found."));

        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && details.CancelledByUserId != currentUserId)
        {
            var booking = await _bookingService.GetByIdAsync(bookingId);
            if (booking != null && booking.UserId != currentUserId)
            {
                return Forbid();
            }
        }

        return Ok(ApiResponse<CancellationResponseDto>.Ok(details));
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
