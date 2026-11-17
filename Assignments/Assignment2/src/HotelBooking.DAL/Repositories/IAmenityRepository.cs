namespace HotelBooking.DAL.Repositories;

using HotelBooking.Models.DTOs;

public interface IAmenityRepository
{
    Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto);
    Task<List<AmenityResponseDto>> GetAllAsync(bool includeInactive = false);
    Task<AmenityResponseDto?> GetByIdAsync(int amenityId);
    Task<AmenityResponseDto> UpdateAsync(int amenityId, UpdateAmenityDto dto);
    Task<bool> SoftDeleteAsync(int amenityId);
    Task<List<AmenityResponseDto>> AssignAmenitiesToRoomAsync(int roomId, IEnumerable<int> amenityIds);
    Task<List<AmenityResponseDto>> GetByRoomIdAsync(int roomId);
    Task<bool> RemoveAmenityFromRoomAsync(int roomId, int amenityId);
}
