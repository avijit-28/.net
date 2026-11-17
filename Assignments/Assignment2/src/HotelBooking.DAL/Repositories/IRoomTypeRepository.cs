namespace HotelBooking.DAL.Repositories;

using HotelBooking.Models.Entities;

public interface IRoomTypeRepository
{
    Task<RoomType> CreateAsync(RoomType roomType);
    Task<List<RoomType>> GetAllAsync(bool includeInactive = false);
    Task<RoomType?> GetByIdAsync(int roomTypeId);
    Task<RoomType> UpdateAsync(RoomType roomType);
    Task<bool> SoftDeleteAsync(int roomTypeId);
}
