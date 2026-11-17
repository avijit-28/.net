namespace HotelBooking.BAL.Services;

using HotelBooking.BAL.Security;
using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;
using HotelBooking.Models.Entities;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserProfileDto> RegisterAsync(RegisterRequestDto request)
    {
        var (hash, salt) = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = "Customer"
        };

        var created = await _userRepository.RegisterAsync(user, hash, salt);
        return MapProfile(created);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailForLoginAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password, or account is deactivated.");
        }

        bool isValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return MapProfile(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto request)
    {
        var updated = await _userRepository.UpdateProfileAsync(userId, request.FullName, request.PhoneNumber);
        return MapProfile(updated);
    }

    public async Task<UserProfileDto> ToggleStatusAsync(int userId, bool isActive, int adminUserId)
    {
        var updated = await _userRepository.ToggleStatusAsync(userId, isActive, adminUserId);
        return MapProfile(updated);
    }

    private static UserProfileDto MapProfile(User user) => new()
    {
        UserId = user.UserId,
        FullName = user.FullName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Role = user.Role,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}
