namespace HotelBooking.Models.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; } = 1;
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = "Confirmed";
    public string? CustomerNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
