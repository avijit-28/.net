namespace HotelBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

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
