namespace HotelBooking.DAL.Repositories;

using System.Data;
using HotelBooking.DAL.Connection;
using HotelBooking.Models.DTOs;
using Microsoft.Data.SqlClient;

public interface ICancellationRepository
{
    Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason);
    Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId);
}

public class CancellationRepository : ICancellationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CancellationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Booking_Cancel", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.Int) { Value = bookingId });
        command.Parameters.Add(new SqlParameter("@CancelledByUserId", SqlDbType.Int) { Value = cancelledByUserId });
        command.Parameters.Add(new SqlParameter("@CancellationReason", SqlDbType.NVarChar, 500) { Value = (object?)reason ?? DBNull.Value });

        var cancellationIdParam = new SqlParameter("@CancellationId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var refundPctParam = new SqlParameter("@RefundPercentage", SqlDbType.Decimal) { Precision = 5, Scale = 2, Direction = ParameterDirection.Output };
        var refundAmtParam = new SqlParameter("@RefundAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
        var cancelChargeParam = new SqlParameter("@CancellationCharge", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };

        command.Parameters.Add(cancellationIdParam);
        command.Parameters.Add(refundPctParam);
        command.Parameters.Add(refundAmtParam);
        command.Parameters.Add(cancelChargeParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new CancellationResponseDto
            {
                CancellationId = reader.GetInt32(reader.GetOrdinal("CancellationId")),
                BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                CancelledAt = reader.GetDateTime(reader.GetOrdinal("CancelledAt")),
                CancellationReason = reader.IsDBNull(reader.GetOrdinal("CancellationReason")) ? null : reader.GetString(reader.GetOrdinal("CancellationReason")),
                RefundPercentage = reader.GetDecimal(reader.GetOrdinal("RefundPercentage")),
                RefundAmount = reader.GetDecimal(reader.GetOrdinal("RefundAmount")),
                CancellationCharge = reader.GetDecimal(reader.GetOrdinal("CancellationCharge")),
                CancelledByUserId = reader.GetInt32(reader.GetOrdinal("CancelledByUserId")),
                OriginalBookingAmount = reader.GetDecimal(reader.GetOrdinal("OriginalBookingAmount")),
                HoursBeforeCheckIn = reader.GetInt32(reader.GetOrdinal("HoursBeforeCheckIn"))
            };
        }

        throw new InvalidOperationException("Failed to cancel booking.");
    }

    public async Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_Cancellation_GetDetails", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.Int) { Value = bookingId });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new CancellationResponseDto
            {
                CancellationId = reader.GetInt32(reader.GetOrdinal("CancellationId")),
                BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                CancelledAt = reader.GetDateTime(reader.GetOrdinal("CancelledAt")),
                CancellationReason = reader.IsDBNull(reader.GetOrdinal("CancellationReason")) ? null : reader.GetString(reader.GetOrdinal("CancellationReason")),
                RefundPercentage = reader.GetDecimal(reader.GetOrdinal("RefundPercentage")),
                RefundAmount = reader.GetDecimal(reader.GetOrdinal("RefundAmount")),
                CancellationCharge = reader.GetDecimal(reader.GetOrdinal("CancellationCharge")),
                CancelledByUserId = reader.GetInt32(reader.GetOrdinal("CancelledByUserId")),
                OriginalBookingAmount = reader.GetDecimal(reader.GetOrdinal("OriginalBookingAmount"))
            };
        }

        return null;
    }
}
