namespace HotelBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

// -----------------------------------------------------------------------------
// Authentication & User DTOs
// -----------------------------------------------------------------------------
public class RegisterRequestDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? PhoneNumber { get; set; }
}

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class UserProfileDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateProfileDto
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? PhoneNumber { get; set; }
}

public class ToggleUserStatusDto
{
    public bool IsActive { get; set; }
}

// -----------------------------------------------------------------------------
// Room Type DTOs
// -----------------------------------------------------------------------------
public class CreateRoomTypeDto
{
    [Required, StringLength(50)]
    public string TypeName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 100000.00)]
    public decimal BasePrice { get; set; }

    [Range(1, 20)]
    public int MaxOccupancy { get; set; }
}

public class UpdateRoomTypeDto
{
    [Required, StringLength(50)]
    public string TypeName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 100000.00)]
    public decimal BasePrice { get; set; }

    [Range(1, 20)]
    public int MaxOccupancy { get; set; }

    public bool IsActive { get; set; }
}

public class RoomTypeResponseDto
{
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int MaxOccupancy { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// -----------------------------------------------------------------------------
// Room DTOs
// -----------------------------------------------------------------------------
public class CreateRoomDto
{
    [Required, StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    public int RoomTypeId { get; set; }

    public int FloorNumber { get; set; } = 1;

    public string Status { get; set; } = "Available";
}

public class UpdateRoomDto
{
    [Required, StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    public int RoomTypeId { get; set; }

    public int FloorNumber { get; set; }

    [Required]
    public string Status { get; set; } = "Available";

    public bool IsActive { get; set; }
}

public class UpdateRoomStatusDto
{
    [Required]
    public string Status { get; set; } = "Available";
}

public class RoomResponseDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int MaxOccupancy { get; set; }
}

public class RoomDetailResponseDto : RoomResponseDto
{
    public string? RoomTypeDescription { get; set; }
    public List<AmenityResponseDto> Amenities { get; set; } = new();
}

// -----------------------------------------------------------------------------
// Amenity DTOs
// -----------------------------------------------------------------------------
public class CreateAmenityDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? IconCode { get; set; }
}

public class UpdateAmenityDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? IconCode { get; set; }

    public bool IsActive { get; set; }
}

public class AmenityResponseDto
{
    public int AmenityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
}

public class AssignAmenitiesDto
{
    [Required]
    public List<int> AmenityIds { get; set; } = new();
}

// -----------------------------------------------------------------------------
// Search & Pagination DTOs
// -----------------------------------------------------------------------------
public class RoomSearchQueryDto
{
    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    public int? Guests { get; set; }
    public int? RoomTypeId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? AmenityIds { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class RoomSearchResultDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public string? RoomTypeDescription { get; set; }
    public decimal BasePricePerNight { get; set; }
    public decimal TotalStayPrice { get; set; }
    public int StayNights { get; set; }
    public int MaxOccupancy { get; set; }
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 0;
}

// -----------------------------------------------------------------------------
// Booking DTOs
// -----------------------------------------------------------------------------
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

// -----------------------------------------------------------------------------
// Cancellation DTOs
// -----------------------------------------------------------------------------
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

// -----------------------------------------------------------------------------
// Generic API Envelope DTO
// -----------------------------------------------------------------------------
public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message, Data = default };
}
