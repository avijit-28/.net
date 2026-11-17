namespace HotelBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CancelBookingRequestDto
{
    [StringLength(500)]
    public string? Reason { get; set; }
}

public class CancellationResponseDto
{
    public int CancellationId { get; set; }
    public int BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public decimal RefundPercentage { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal CancellationCharge { get; set; }
    public int CancelledByUserId { get; set; }
    public decimal OriginalBookingAmount { get; set; }
    public int HoursBeforeCheckIn { get; set; }
}
