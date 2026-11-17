namespace HotelBooking.BAL.Services;

using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;

public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _repository;

    public AmenityService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto) => _repository.CreateAsync(dto);
    public Task<List<AmenityResponseDto>> GetAllAsync(bool includeInactive = false) => _repository.GetAllAsync(includeInactive);
    public Task<AmenityResponseDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<AmenityResponseDto> UpdateAsync(int id, UpdateAmenityDto dto) => _repository.UpdateAsync(id, dto);
    public Task<bool> SoftDeleteAsync(int id) => _repository.SoftDeleteAsync(id);
    public Task<List<AmenityResponseDto>> AssignAmenitiesToRoomAsync(int roomId, List<int> amenityIds) => _repository.AssignAmenitiesToRoomAsync(roomId, amenityIds);
    public Task<List<AmenityResponseDto>> GetByRoomIdAsync(int roomId) => _repository.GetByRoomIdAsync(roomId);
    public Task<bool> RemoveAmenityFromRoomAsync(int roomId, int amenityId) => _repository.RemoveAmenityFromRoomAsync(roomId, amenityId);
}
