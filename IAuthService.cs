using DaNangSafeMap.Models;

namespace DaNangSafeMap.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<(bool Success, string Message)> RegisterUserAsync(RegisterViewModel model);
        string HashPassword(string password, string salt);
    }
}