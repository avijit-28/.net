using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    InventoryRepository repository,
    IPasswordHasher<PasswordHashSubject> passwordHasher,
    JwtSettings settings) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var hash = await repository.GetPasswordHashAsync(request.Username, cancellationToken);
        if (hash is null || passwordHasher.VerifyHashedPassword(new PasswordHashSubject(), hash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid username or password." });

        var expires = DateTime.UtcNow.AddMinutes(settings.ExpiresMinutes);
        var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims, expires: expires, signingCredentials: credentials);
        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expires));
    }
}
