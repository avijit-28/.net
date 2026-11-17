namespace HotelBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

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
