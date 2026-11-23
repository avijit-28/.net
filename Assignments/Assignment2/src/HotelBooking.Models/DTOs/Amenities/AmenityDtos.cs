namespace HotelBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CreateAmenityDto
{
    private string _name = string.Empty;

    [Required(ErrorMessage = "The Name field is required."), StringLength(100)]
    public string Name
    {
        get => _name;
        set { if (!string.IsNullOrWhiteSpace(value)) _name = value; }
    }

    [JsonPropertyName("amenityName")]
    public string? AmenityName
    {
        get => _name;
        set { if (!string.IsNullOrWhiteSpace(value)) _name = value; }
    }

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? IconCode { get; set; }
}

public class UpdateAmenityDto
{
    private string _name = string.Empty;

    [Required(ErrorMessage = "The Name field is required."), StringLength(100)]
    public string Name
    {
        get => _name;
        set { if (!string.IsNullOrWhiteSpace(value)) _name = value; }
    }

    [JsonPropertyName("amenityName")]
    public string? AmenityName
    {
        get => _name;
        set { if (!string.IsNullOrWhiteSpace(value)) _name = value; }
    }

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
