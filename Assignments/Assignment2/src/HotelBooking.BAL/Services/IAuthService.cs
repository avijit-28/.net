namespace HotelBooking.BAL.Services;

using HotelBooking.Models.DTOs;

public interface IAuthService
{
    Task<UserProfileDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto request);
    Task<UserProfileDto> ToggleStatusAsync(int userId, bool isActive, int adminUserId);
}
