namespace HotelBooking.BAL.Services;

using HotelBooking.Models.DTOs;

public interface IRoomService
{
    Task<RoomResponseDto> CreateAsync(CreateRoomDto dto);
    Task<List<RoomResponseDto>> GetAllAsync(int? roomTypeId = null, string? status = null, bool includeInactive = false);
    Task<RoomDetailResponseDto?> GetByIdAsync(int roomId);
    Task<RoomResponseDto> UpdateAsync(int roomId, UpdateRoomDto dto);
    Task<bool> UpdateStatusAsync(int roomId, string status);
    Task<bool> SoftDeleteAsync(int roomId);
    Task<PagedResultDto<RoomSearchResultDto>> SearchAvailableAsync(RoomSearchQueryDto query);
}
