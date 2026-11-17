namespace HotelBooking.DAL.Repositories;

using System.Data;
using HotelBooking.DAL.Connection;
using HotelBooking.Models.Entities;
using Microsoft.Data.SqlClient;

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RoomTypeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RoomType> CreateAsync(RoomType roomType)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomType_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@TypeName", SqlDbType.NVarChar, 50) { Value = roomType.TypeName });
        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object?)roomType.Description ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@BasePrice", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = roomType.BasePrice });
        command.Parameters.Add(new SqlParameter("@MaxOccupancy", SqlDbType.Int) { Value = roomType.MaxOccupancy });

        var createdIdParam = new SqlParameter("@CreatedRoomTypeId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(createdIdParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRoomType(reader);
        }

        throw new InvalidOperationException("Failed to create RoomType.");
    }

    public async Task<List<RoomType>> GetAllAsync(bool includeInactive = false)
    {
        var list = new List<RoomType>();
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomType_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@IncludeInactive", SqlDbType.Bit) { Value = includeInactive });

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapRoomType(reader));
        }

        return list;
    }

    public async Task<RoomType?> GetByIdAsync(int roomTypeId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomType_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = roomTypeId });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRoomType(reader);
        }

        return null;
    }

    public async Task<RoomType> UpdateAsync(RoomType roomType)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomType_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = roomType.RoomTypeId });
        command.Parameters.Add(new SqlParameter("@TypeName", SqlDbType.NVarChar, 50) { Value = roomType.TypeName });
        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object?)roomType.Description ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@BasePrice", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = roomType.BasePrice });
        command.Parameters.Add(new SqlParameter("@MaxOccupancy", SqlDbType.Int) { Value = roomType.MaxOccupancy });
        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = roomType.IsActive });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRoomType(reader);
        }

        throw new InvalidOperationException("Failed to update RoomType.");
    }

    public async Task<bool> SoftDeleteAsync(int roomTypeId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_RoomType_SoftDelete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = roomTypeId });

        using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    private static RoomType MapRoomType(SqlDataReader reader)
    {
        return new RoomType
        {
            RoomTypeId = reader.GetInt32(reader.GetOrdinal("RoomTypeId")),
            TypeName = reader.GetString(reader.GetOrdinal("TypeName")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
            BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
            MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }
}
