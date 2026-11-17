namespace HotelBooking.DAL.Repositories;

using System.Data;
using HotelBooking.DAL.Connection;
using HotelBooking.Models.DTOs;
using HotelBooking.Models.Entities;
using Microsoft.Data.SqlClient;

public class AmenityRepository : IAmenityRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AmenityRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Amenity_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = dto.Name });
        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255) { Value = (object?)dto.Description ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@IconCode", SqlDbType.NVarChar, 50) { Value = (object?)dto.IconCode ?? DBNull.Value });

        var createdIdParam = new SqlParameter("@CreatedAmenityId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(createdIdParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapAmenity(reader);
        }

        throw new InvalidOperationException("Failed to create Amenity.");
    }

    public async Task<List<AmenityResponseDto>> GetAllAsync(bool includeInactive = false)
    {
        var list = new List<AmenityResponseDto>();
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Amenity_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@IncludeInactive", SqlDbType.Bit) { Value = includeInactive });

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapAmenity(reader));
        }

        return list;
    }

    public async Task<AmenityResponseDto?> GetByIdAsync(int amenityId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Amenity_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@AmenityId", SqlDbType.Int) { Value = amenityId });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapAmenity(reader);
        }

        return null;
    }

    public async Task<AmenityResponseDto> UpdateAsync(int amenityId, UpdateAmenityDto dto)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Amenity_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@AmenityId", SqlDbType.Int) { Value = amenityId });
        command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = dto.Name });
        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255) { Value = (object?)dto.Description ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@IconCode", SqlDbType.NVarChar, 50) { Value = (object?)dto.IconCode ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = dto.IsActive });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapAmenity(reader);
        }

        throw new InvalidOperationException("Failed to update Amenity.");
    }

    public async Task<bool> SoftDeleteAsync(int amenityId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Amenity_SoftDelete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@AmenityId", SqlDbType.Int) { Value = amenityId });

        using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<List<AmenityResponseDto>> AssignAmenitiesToRoomAsync(int roomId, IEnumerable<int> amenityIds)
    {
        var list = new List<AmenityResponseDto>();
        string commaSeparated = string.Join(",", amenityIds);

        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomAmenities_Assign", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });
        command.Parameters.Add(new SqlParameter("@AmenityIds", SqlDbType.NVarChar, -1) { Value = commaSeparated });

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new AmenityResponseDto
            {
                AmenityId = reader.GetInt32(reader.GetOrdinal("AmenityId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IconCode = reader.IsDBNull(reader.GetOrdinal("IconCode")) ? null : reader.GetString(reader.GetOrdinal("IconCode")),
                IsActive = true,
                AssignedAt = reader.GetDateTime(reader.GetOrdinal("AssignedAt"))
            });
        }

        return list;
    }

    public async Task<List<AmenityResponseDto>> GetByRoomIdAsync(int roomId)
    {
        var list = new List<AmenityResponseDto>();
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomAmenities_GetByRoomId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new AmenityResponseDto
            {
                AmenityId = reader.GetInt32(reader.GetOrdinal("AmenityId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IconCode = reader.IsDBNull(reader.GetOrdinal("IconCode")) ? null : reader.GetString(reader.GetOrdinal("IconCode")),
                IsActive = true,
                AssignedAt = reader.GetDateTime(reader.GetOrdinal("AssignedAt"))
            });
        }

        return list;
    }

    public async Task<bool> RemoveAmenityFromRoomAsync(int roomId, int amenityId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomAmenities_Remove", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });
        command.Parameters.Add(new SqlParameter("@AmenityId", SqlDbType.Int) { Value = amenityId });

        using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    private static AmenityResponseDto MapAmenity(SqlDataReader reader)
    {
        return new AmenityResponseDto
        {
            AmenityId = reader.GetInt32(reader.GetOrdinal("AmenityId")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
            IconCode = reader.IsDBNull(reader.GetOrdinal("IconCode")) ? null : reader.GetString(reader.GetOrdinal("IconCode")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}
