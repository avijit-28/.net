namespace HotelBooking.BAL.Services;

using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repository;

    public RoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public Task<RoomResponseDto> CreateAsync(CreateRoomDto dto) => _repository.CreateAsync(dto);
    public Task<List<RoomResponseDto>> GetAllAsync(int? roomTypeId = null, string? status = null, bool includeInactive = false) => _repository.GetAllAsync(roomTypeId, status, includeInactive);
    public Task<RoomDetailResponseDto?> GetByIdAsync(int roomId) => _repository.GetByIdAsync(roomId);
    public Task<RoomResponseDto> UpdateAsync(int roomId, UpdateRoomDto dto) => _repository.UpdateAsync(roomId, dto);
    public Task<bool> UpdateStatusAsync(int roomId, string status) => _repository.UpdateStatusAsync(roomId, status);
    public Task<bool> SoftDeleteAsync(int roomId) => _repository.SoftDeleteAsync(roomId);
    public Task<PagedResultDto<RoomSearchResultDto>> SearchAvailableAsync(RoomSearchQueryDto query) => _repository.SearchAvailableAsync(query);
}
