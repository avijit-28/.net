namespace HotelBooking.DAL.Repositories;

using System.Data;
using HotelBooking.DAL.Connection;
using HotelBooking.Models.Entities;
using Microsoft.Data.SqlClient;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User> RegisterAsync(User user, string passwordHash, string passwordSalt)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_User_Register", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = user.FullName });
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = user.Email });
        command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = passwordHash });
        command.Parameters.Add(new SqlParameter("@PasswordSalt", SqlDbType.NVarChar, 128) { Value = passwordSalt });
        command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 20) { Value = (object?)user.PhoneNumber ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role });

        var createdIdParam = new SqlParameter("@CreatedUserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(createdIdParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }

        throw new InvalidOperationException("Failed to register user record.");
    }

    public async Task<User?> GetByEmailForLoginAsync(string email)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_User_Login", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = email });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                Role = reader.GetString(reader.GetOrdinal("Role")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        return null;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_User_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }

        return null;
    }

    public async Task<User> UpdateProfileAsync(int userId, string fullName, string? phoneNumber)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_User_UpdateProfile", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullName });
        command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 20) { Value = (object?)phoneNumber ?? DBNull.Value });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }

        throw new InvalidOperationException("Failed to update user profile.");
    }

    public async Task<User> ToggleStatusAsync(int userId, bool isActive, int adminUserId)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        using var command = new SqlCommand("dbo.usp_User_ToggleStatus", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive });
        command.Parameters.Add(new SqlParameter("@AdminUserId", SqlDbType.Int) { Value = adminUserId });

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }

        throw new InvalidOperationException("Failed to toggle user status.");
    }

    private static User MapUser(SqlDataReader reader)
    {
        int updatedAtIndex = -1;
        try { updatedAtIndex = reader.GetOrdinal("UpdatedAt"); } catch (IndexOutOfRangeException) { }

        return new User
        {
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
            Role = reader.GetString(reader.GetOrdinal("Role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = (updatedAtIndex >= 0 && !reader.IsDBNull(updatedAtIndex)) ? reader.GetDateTime(updatedAtIndex) : null
        };
    }
}
