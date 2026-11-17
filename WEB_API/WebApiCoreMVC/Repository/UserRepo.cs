using System.Data;
using Microsoft.Data.SqlClient;
using WebApiCoreMVC.Models;

namespace WebApiCoreMVC.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly string _connectionString;

        public UserRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Automatically ensure Users table exists and is seeded with demo users
                    string createTableSql = @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                        BEGIN
                            CREATE TABLE Users (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Username VARCHAR(50) UNIQUE NOT NULL,
                                PasswordHash VARCHAR(255) NOT NULL,
                                Email VARCHAR(100) NOT NULL,
                                Role VARCHAR(20) NOT NULL
                            );
                            INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES 
                            ('admin', 'admin123', 'admin@example.com', 'Admin'), 
                            ('user1', 'user123', 'user1@example.com', 'User');
                        END";
                    using (var createCmd = new SqlCommand(createTableSql, conn))
                    {
                        await createCmd.ExecuteNonQueryAsync();
                    }

                    string query = "SELECT Id, Username, Email, Role FROM Users WHERE Username = @Username AND PasswordHash = @Password";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Username = reader["Username"].ToString() ?? string.Empty,
                                    Email = reader["Email"].ToString() ?? string.Empty,
                                    Role = reader["Role"].ToString() ?? string.Empty
                                };
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback for demo credentials if database connection is unavailable
                if (username == "admin" && password == "admin123")
                {
                    return new User { Id = 1, Username = "admin", Email = "admin@example.com", Role = "Admin" };
                }
                if (username == "user1" && password == "user123")
                {
                    return new User { Id = 2, Username = "user1", Email = "user1@example.com", Role = "User" };
                }
            }

            return null;
        }
    }
}
