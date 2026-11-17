namespace HotelBooking.BAL.Services;

using HotelBooking.Models.DTOs;

public interface IRoomTypeService
{
    Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto);
    Task<List<RoomTypeResponseDto>> GetAllAsync(bool includeInactive = false);
    Task<RoomTypeResponseDto?> GetByIdAsync(int id);
    Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto);
    Task<bool> SoftDeleteAsync(int id);
}
