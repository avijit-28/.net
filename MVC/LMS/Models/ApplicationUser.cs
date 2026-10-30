using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            if (!string.IsNullOrEmpty(FullName))
            {
                userIdentity.AddClaim(new Claim("FullName", FullName));
            }
            return userIdentity;
        }
    }
}
