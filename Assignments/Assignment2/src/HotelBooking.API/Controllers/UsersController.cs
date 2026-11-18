namespace HotelBooking.API.Controllers;

using System.Security.Claims;
using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Manages user registration, authentication, profile updates, and role-based activation status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new customer account with salted PBKDF2 password hashing.
    /// </summary>
    /// <param name="request">Customer registration details including email, password, and phone number.</param>
    /// <returns>Created user profile with HTTP 201 Created.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var profile = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = profile.UserId }, ApiResponse<UserProfileDto>.Ok(profile, "User registered successfully."));
    }

    /// <summary>
    /// Authenticates a user and generates a signed JWT token containing UserId, Email, and Role claims.
    /// </summary>
    /// <param name="request">Login credentials (email and password).</param>
    /// <returns>JWT bearer token and user metadata.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var authResult = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponseDto>.Ok(authResult, "Login successful."));
    }

    /// <summary>
    /// Retrieves user profile details by ID. Restricted to the account owner or an Administrator.
    /// </summary>
    /// <param name="id">Target user ID.</param>
    /// <returns>User profile information.</returns>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetById(int id)
    {
        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && currentUserId != id)
        {
            return Forbid();
        }

        var profile = await _authService.GetProfileAsync(id);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    /// <summary>
    /// Updates the full name and phone number of the specified user. Restricted to account owner or Admin.
    /// </summary>
    /// <param name="id">User ID to update.</param>
    /// <param name="request">Updated full name and phone number.</param>
    /// <returns>Updated user profile.</returns>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile(int id, [FromBody] UpdateProfileDto request)
    {
        int currentUserId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        if (role != "Admin" && currentUserId != id)
        {
            return Forbid();
        }

        var updated = await _authService.UpdateProfileAsync(id, request);
        return Ok(ApiResponse<UserProfileDto>.Ok(updated, "Profile updated successfully."));
    }

    /// <summary>
    /// Activates or deactivates a user account. Restricted to Administrators only.
    /// </summary>
    /// <param name="id">Target user ID.</param>
    /// <param name="isActive">Active status flag (true = active, false = deactivated).</param>
    /// <returns>Updated user profile.</returns>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> ToggleStatus(int id, [FromQuery] bool isActive)
    {
        int adminUserId = GetCurrentUserId();
        var result = await _authService.ToggleStatusAsync(id, isActive, adminUserId);
        return Ok(ApiResponse<UserProfileDto>.Ok(result, $"User status updated to {(isActive ? "Active" : "Inactive")}."));
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : 0;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
