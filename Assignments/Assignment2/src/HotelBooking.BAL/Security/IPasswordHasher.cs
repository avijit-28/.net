namespace HotelBooking.BAL.Security;

public interface IPasswordHasher
{
    (string Hash, string Salt) HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string storedHash, string storedSalt);
}
