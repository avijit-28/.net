namespace HotelBooking.BAL.Services;

using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;
using HotelBooking.Models.Entities;

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _repository;

    public RoomTypeService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto)
    {
        var entity = new RoomType
        {
            TypeName = dto.TypeName,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            MaxOccupancy = dto.MaxOccupancy
        };
        var created = await _repository.CreateAsync(entity);
        return MapDto(created);
    }

    public async Task<List<RoomTypeResponseDto>> GetAllAsync(bool includeInactive = false)
    {
        var list = await _repository.GetAllAsync(includeInactive);
        return list.Select(MapDto).ToList();
    }

    public async Task<RoomTypeResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MapDto(entity);
    }

    public async Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto)
    {
        var entity = new RoomType
        {
            RoomTypeId = id,
            TypeName = dto.TypeName,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            MaxOccupancy = dto.MaxOccupancy,
            IsActive = dto.IsActive
        };
        var updated = await _repository.UpdateAsync(entity);
        return MapDto(updated);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        return await _repository.SoftDeleteAsync(id);
    }

    private static RoomTypeResponseDto MapDto(RoomType e) => new()
    {
        RoomTypeId = e.RoomTypeId,
        TypeName = e.TypeName,
        Description = e.Description,
        BasePrice = e.BasePrice,
        MaxOccupancy = e.MaxOccupancy,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
