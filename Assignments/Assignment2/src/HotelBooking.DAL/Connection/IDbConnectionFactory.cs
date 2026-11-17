namespace HotelBooking.DAL.Connection;

using Microsoft.Data.SqlClient;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
    Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
