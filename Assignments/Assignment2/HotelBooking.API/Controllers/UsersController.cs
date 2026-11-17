namespace HotelBooking.API.Controllers;

using System.Security.Claims;
using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var profile = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = profile.UserId }, ApiResponse<UserProfileDto>.Ok(profile, "User registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var authResult = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponseDto>.Ok(authResult, "Login successful."));
    }

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

        var profile = await _authService.UpdateProfileAsync(id, request);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Profile updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> ToggleStatus(int id, [FromBody] ToggleUserStatusDto request)
    {
        int adminUserId = GetCurrentUserId();
        var profile = await _authService.ToggleStatusAsync(id, request.IsActive, adminUserId);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile, "User status updated successfully."));
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
