namespace HotelBooking.BAL.Security;

using HotelBooking.Models.Entities;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
