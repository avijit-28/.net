using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiCoreMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        // GET: api/data/secure (Requires any valid JWT Token)
        [Authorize]
        [HttpGet("secure")]
        public IActionResult GetSecureData()
        {
            // Extract identity claims from the incoming validated JWT token
            var username = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst("UserId")?.Value;

            return Ok(new
            {
                message = $"Authorization verified. Welcome back, {username}!",
                profile = new
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    Role = role
                }
            });
        }

        // GET: api/data/admin (Only accessible if Role claim is 'Admin')
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult GetAdminData()
        {
            return Ok(new
            {
                message = "Welcome Administrator! You have authorization for this endpoint."
            });
        }
    }
}
