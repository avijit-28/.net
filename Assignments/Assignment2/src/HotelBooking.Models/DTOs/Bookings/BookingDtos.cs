namespace HotelBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateBookingRequestDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Range(1, 20)]
    public int NumberOfGuests { get; set; } = 1;

    [StringLength(500)]
    public string? CustomerNotes { get; set; }
}

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal NightlyRate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public string? CustomerNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingDetailDto : BookingResponseDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
}
