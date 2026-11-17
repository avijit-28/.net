namespace HotelBooking.Models.Entities;

public class Cancellation
{
    public int CancellationId { get; set; }
    public int BookingId { get; set; }
    public int CancelledByUserId { get; set; }
    public string? CancellationReason { get; set; }
    public decimal RefundPercentage { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal CancellationCharge { get; set; }
    public DateTime CancelledAt { get; set; }
}
