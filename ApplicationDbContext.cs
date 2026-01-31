using DaNangSafeMap.Models;
using Microsoft.EntityFrameworkCore;

namespace DaNangSafeMap.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
    }
}