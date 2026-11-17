namespace HotelBooking.BAL.Services;

using HotelBooking.Models.DTOs;

public interface IAmenityService
{
    Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto);
    Task<List<AmenityResponseDto>> GetAllAsync(bool includeInactive = false);
    Task<AmenityResponseDto?> GetByIdAsync(int id);
    Task<AmenityResponseDto> UpdateAsync(int id, UpdateAmenityDto dto);
    Task<bool> SoftDeleteAsync(int id);
    Task<List<AmenityResponseDto>> AssignAmenitiesToRoomAsync(int roomId, List<int> amenityIds);
    Task<List<AmenityResponseDto>> GetByRoomIdAsync(int roomId);
    Task<bool> RemoveAmenityFromRoomAsync(int roomId, int amenityId);
}
