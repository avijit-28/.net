namespace HotelBooking.DAL.Repositories;

using System.Data;
using HotelBooking.DAL.Connection;
using HotelBooking.Models.DTOs;
using Microsoft.Data.SqlClient;

public class RoomRepository : IRoomRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RoomRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomNumber", SqlDbType.NVarChar, 20) { Value = dto.RoomNumber });
        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = dto.RoomTypeId });
        command.Parameters.Add(new SqlParameter("@FloorNumber", SqlDbType.Int) { Value = dto.FloorNumber });
        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 30) { Value = dto.Status });

        var createdIdParam = new SqlParameter("@CreatedRoomId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(createdIdParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRoomResponse(reader);
        }

        throw new InvalidOperationException("Failed to create Room.");
    }

    public async Task<List<RoomResponseDto>> GetAllAsync(int? roomTypeId = null, string? status = null, bool includeInactive = false)
    {
        var list = new List<RoomResponseDto>();
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = (object?)roomTypeId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 30) { Value = (object?)status ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@IncludeInactive", SqlDbType.Bit) { Value = includeInactive });

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapRoomResponse(reader));
        }

        return list;
    }

    public async Task<RoomDetailResponseDto?> GetByIdAsync(int roomId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });

        using var reader = await command.ExecuteReaderAsync();

        // Result Set 1: Room & RoomType Details
        if (!await reader.ReadAsync())
        {
            return null;
        }

        var detail = new RoomDetailResponseDto
        {
            RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
            RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
            FloorNumber = reader.GetInt32(reader.GetOrdinal("FloorNumber")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            RoomTypeId = reader.GetInt32(reader.GetOrdinal("RoomTypeId")),
            RoomTypeName = reader.GetString(reader.GetOrdinal("TypeName")),
            RoomTypeDescription = reader.IsDBNull(reader.GetOrdinal("RoomTypeDescription")) ? null : reader.GetString(reader.GetOrdinal("RoomTypeDescription")),
            BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
            MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
        };

        // Result Set 2: Associated Amenities
        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                detail.Amenities.Add(new AmenityResponseDto
                {
                    AmenityId = reader.GetInt32(reader.GetOrdinal("AmenityId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    IconCode = reader.IsDBNull(reader.GetOrdinal("IconCode")) ? null : reader.GetString(reader.GetOrdinal("IconCode")),
                    IsActive = true,
                    AssignedAt = reader.GetDateTime(reader.GetOrdinal("AssignedAt"))
                });
            }
        }

        return detail;
    }

    public async Task<RoomResponseDto> UpdateAsync(int roomId, UpdateRoomDto dto)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });
        command.Parameters.Add(new SqlParameter("@RoomNumber", SqlDbType.NVarChar, 20) { Value = dto.RoomNumber });
        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = dto.RoomTypeId });
        command.Parameters.Add(new SqlParameter("@FloorNumber", SqlDbType.Int) { Value = dto.FloorNumber });
        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 30) { Value = dto.Status });
        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = dto.IsActive });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRoomResponse(reader);
        }

        throw new InvalidOperationException("Failed to update Room.");
    }

    public async Task<bool> UpdateStatusAsync(int roomId, string status)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_UpdateStatus", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });
        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 30) { Value = status });

        using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<bool> SoftDeleteAsync(int roomId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_SoftDelete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });

        using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<PagedResultDto<RoomSearchResultDto>> SearchAvailableAsync(RoomSearchQueryDto query)
    {
        var result = new PagedResultDto<RoomSearchResultDto>
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Room_SearchAvailable", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@CheckInDate", SqlDbType.Date) { Value = query.CheckInDate });
        command.Parameters.Add(new SqlParameter("@CheckOutDate", SqlDbType.Date) { Value = query.CheckOutDate });
        command.Parameters.Add(new SqlParameter("@Guests", SqlDbType.Int) { Value = (object?)query.Guests ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = (object?)query.RoomTypeId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@MinPrice", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = (object?)query.MinPrice ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@MaxPrice", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = (object?)query.MaxPrice ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@AmenityIds", SqlDbType.NVarChar, -1) { Value = (object?)query.AmenityIds ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = query.PageNumber });
        command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = query.PageSize });

        var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(totalRecordsParam);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Items.Add(new RoomSearchResultDto
            {
                RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                FloorNumber = reader.GetInt32(reader.GetOrdinal("FloorNumber")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                RoomTypeId = reader.GetInt32(reader.GetOrdinal("RoomTypeId")),
                RoomTypeName = reader.GetString(reader.GetOrdinal("RoomTypeName")),
                RoomTypeDescription = reader.IsDBNull(reader.GetOrdinal("RoomTypeDescription")) ? null : reader.GetString(reader.GetOrdinal("RoomTypeDescription")),
                BasePricePerNight = reader.GetDecimal(reader.GetOrdinal("BasePricePerNight")),
                TotalStayPrice = reader.GetDecimal(reader.GetOrdinal("TotalStayPrice")),
                StayNights = reader.GetInt32(reader.GetOrdinal("StayNights")),
                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
            });
        }

        await reader.CloseAsync();
        result.TotalRecords = totalRecordsParam.Value is int total ? total : 0;

        return result;
    }

    private static RoomResponseDto MapRoomResponse(SqlDataReader reader)
    {
        return new RoomResponseDto
        {
            RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
            RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
            FloorNumber = reader.GetInt32(reader.GetOrdinal("FloorNumber")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            RoomTypeId = reader.GetInt32(reader.GetOrdinal("RoomTypeId")),
            RoomTypeName = reader.GetString(reader.GetOrdinal("TypeName")),
            BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
            MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
        };
    }
}
