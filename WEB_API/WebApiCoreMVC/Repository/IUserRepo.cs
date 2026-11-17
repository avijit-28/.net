using WebApiCoreMVC.Models;

namespace WebApiCoreMVC.Repository
{
    public interface IUserRepo
    {
        Task<User?> ValidateUserAsync(string username, string password);
    }
}
