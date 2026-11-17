namespace HotelBooking.DAL.Repositories;

using HotelBooking.Models.Entities;

public interface IUserRepository
{
    Task<User> RegisterAsync(User user, string passwordHash, string passwordSalt);
    Task<User?> GetByEmailForLoginAsync(string email);
    Task<User?> GetByIdAsync(int userId);
    Task<User> UpdateProfileAsync(int userId, string fullName, string? phoneNumber);
    Task<User> ToggleStatusAsync(int userId, bool isActive, int adminUserId);
}
