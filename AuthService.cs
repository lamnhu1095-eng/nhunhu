using DaNangSafeMap.Data;
using DaNangSafeMap.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DaNangSafeMap.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    _logger.LogWarning("User '{Username}' not found", username);
                    return null;
                }

                var hashedPassword = HashPassword(password, user.Salt);
                
                if (hashedPassword == user.Password_Hash)
                {
                    _logger.LogInformation("User '{Username}' logged in successfully", username);
                    return user;
                }

                _logger.LogWarning("Invalid password for user '{Username}'", username);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user '{Username}'", username);
                return null;
            }
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                // Kiểm tra username đã tồn tại
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                    return (false, "Tên đăng nhập đã tồn tại");

                // Kiểm tra email đã tồn tại
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                    return (false, "Email đã được sử dụng");

                // Tạo salt và hash password
                var salt = Guid.NewGuid().ToString();
                var hashedPassword = HashPassword(model.Password, salt);

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password_Hash = hashedPassword,
                    Salt = salt,
                    Role = "user",
                    Created_At = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New user registered: {Username}", model.Username);
                return (true, "Đăng ký thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user '{Username}'", model.Username);
                return (false, "Đã xảy ra lỗi trong quá trình đăng ký");
            }
        }

        public string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash).ToLower();
            }
        }
    }
}