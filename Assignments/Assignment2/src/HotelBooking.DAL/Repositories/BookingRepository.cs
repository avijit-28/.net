namespace HotelBooking.DAL.Repositories;

using System.Data;
using HotelBooking.DAL.Connection;
using HotelBooking.Models.DTOs;
using Microsoft.Data.SqlClient;

public class BookingRepository : IBookingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BookingRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<BookingResponseDto> CreateAsync(int userId, CreateBookingRequestDto dto)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Booking_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = dto.RoomId });
        command.Parameters.Add(new SqlParameter("@CheckInDate", SqlDbType.Date) { Value = dto.CheckInDate });
        command.Parameters.Add(new SqlParameter("@CheckOutDate", SqlDbType.Date) { Value = dto.CheckOutDate });
        command.Parameters.Add(new SqlParameter("@NumberOfGuests", SqlDbType.Int) { Value = dto.NumberOfGuests });
        command.Parameters.Add(new SqlParameter("@CustomerNotes", SqlDbType.NVarChar, 500) { Value = (object?)dto.CustomerNotes ?? DBNull.Value });

        var createdIdParam = new SqlParameter("@CreatedBookingId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var generatedRefParam = new SqlParameter("@GeneratedBookingReference", SqlDbType.NVarChar, 30) { Direction = ParameterDirection.Output };
        command.Parameters.Add(createdIdParam);
        command.Parameters.Add(generatedRefParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapBookingResponse(reader);
        }

        throw new InvalidOperationException("Failed to create Booking.");
    }

    public async Task<BookingDetailDto?> GetByIdAsync(int bookingId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Booking_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.Int) { Value = bookingId });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapBookingDetail(reader);
        }

        return null;
    }

    public async Task<BookingDetailDto?> GetByReferenceAsync(string bookingReference)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Booking_GetByReference", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@BookingReference", SqlDbType.NVarChar, 30) { Value = bookingReference });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapBookingDetail(reader);
        }

        return null;
    }

    public async Task<List<BookingResponseDto>> GetByUserAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
        var list = new List<BookingResponseDto>();
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Booking_GetByUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber });
        command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new BookingResponseDto
            {
                BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                UserId = userId,
                RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                RoomTypeName = reader.GetString(reader.GetOrdinal("RoomTypeName")),
                CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                TotalNights = (int)(reader.GetDateTime(reader.GetOrdinal("CheckOutDate")) - reader.GetDateTime(reader.GetOrdinal("CheckInDate"))).TotalDays,
                NumberOfGuests = reader.GetInt32(reader.GetOrdinal("NumberOfGuests")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                BookingStatus = reader.GetString(reader.GetOrdinal("BookingStatus")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return list;
    }

    public async Task<PagedResultDto<BookingDetailDto>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 20)
    {
        var result = new PagedResultDto<BookingDetailDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Booking_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = (object?)status ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = (object?)startDate ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = (object?)endDate ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber });
        command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });

        var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(totalRecordsParam);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Items.Add(new BookingDetailDto
            {
                BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail")),
                RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                RoomTypeName = reader.GetString(reader.GetOrdinal("RoomTypeName")),
                CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                TotalNights = (int)(reader.GetDateTime(reader.GetOrdinal("CheckOutDate")) - reader.GetDateTime(reader.GetOrdinal("CheckInDate"))).TotalDays,
                NumberOfGuests = reader.GetInt32(reader.GetOrdinal("NumberOfGuests")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                BookingStatus = reader.GetString(reader.GetOrdinal("BookingStatus")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        await reader.CloseAsync();
        result.TotalRecords = totalRecordsParam.Value is int total ? total : 0;

        return result;
    }

    private static BookingResponseDto MapBookingResponse(SqlDataReader reader)
    {
        return new BookingResponseDto
        {
            BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
            BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
            RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
            RoomTypeName = reader.GetString(reader.GetOrdinal("RoomTypeName")),
            NightlyRate = reader.GetDecimal(reader.GetOrdinal("NightlyRate")),
            CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
            CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
            TotalNights = reader.GetInt32(reader.GetOrdinal("TotalNights")),
            NumberOfGuests = reader.GetInt32(reader.GetOrdinal("NumberOfGuests")),
            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
            BookingStatus = reader.GetString(reader.GetOrdinal("BookingStatus")),
            CustomerNotes = reader.IsDBNull(reader.GetOrdinal("CustomerNotes")) ? null : reader.GetString(reader.GetOrdinal("CustomerNotes")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    private static BookingDetailDto MapBookingDetail(SqlDataReader reader)
    {
        return new BookingDetailDto
        {
            BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
            BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
            CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail")),
            RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
            RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
            RoomTypeName = reader.GetString(reader.GetOrdinal("RoomTypeName")),
            NightlyRate = reader.GetDecimal(reader.GetOrdinal("NightlyRate")),
            CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
            CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
            TotalNights = reader.GetInt32(reader.GetOrdinal("TotalNights")),
            NumberOfGuests = reader.GetInt32(reader.GetOrdinal("NumberOfGuests")),
            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
            BookingStatus = reader.GetString(reader.GetOrdinal("BookingStatus")),
            CustomerNotes = reader.IsDBNull(reader.GetOrdinal("CustomerNotes")) ? null : reader.GetString(reader.GetOrdinal("CustomerNotes")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }
}
