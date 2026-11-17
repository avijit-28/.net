namespace HotelBooking.Models.Entities;

public class Amenity
{
    public int AmenityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class RoomAmenity
{
    public int RoomAmenityId { get; set; }
    public int RoomId { get; set; }
    public int AmenityId { get; set; }
    public DateTime AssignedAt { get; set; }
}
