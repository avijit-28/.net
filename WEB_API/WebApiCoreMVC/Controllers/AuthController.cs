using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiCoreMVC.Models;
using WebApiCoreMVC.Repository;

namespace WebApiCoreMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepo userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request payload." });
            }

            // Validate against database
            var user = await _userRepo.ValidateUserAsync(login.Username, login.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token = token,
                username = user.Username,
                role = user.Role,
                email = user.Email
            });
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"] 
                ?? "ThisIsSuperSecretKeyThatIsAtLeast32BytesLong!";
            var issuer = _configuration["Jwt:Issuer"] ?? "https://localhost:44321";
            var audience = _configuration["Jwt:Audience"] ?? "https://localhost:44321";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Add claims matching authenticated profile attributes (from PDF slide 20)
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
