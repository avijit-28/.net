using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

if (args.Length == 2 && args[0].Equals("hash-password", StringComparison.OrdinalIgnoreCase))
{
    var hasher = new PasswordHasher<PasswordHashSubject>();
    Console.WriteLine(hasher.HashPassword(new PasswordHashSubject(), args[1]));
    return;
}

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
if (Encoding.UTF8.GetByteCount(jwtSettings.SigningKey) < 32)
    throw new InvalidOperationException("Set Jwt:SigningKey to a secret of at least 32 bytes using User Secrets or an environment variable.");
if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("Inventory")))
    throw new InvalidOperationException("Set ConnectionStrings:Inventory using User Secrets or an environment variable.");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<IPasswordHasher<PasswordHashSubject>, PasswordHasher<PasswordHashSubject>>();
builder.Services.AddScoped<InventoryRepository>();
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public sealed class PasswordHashSubject;
