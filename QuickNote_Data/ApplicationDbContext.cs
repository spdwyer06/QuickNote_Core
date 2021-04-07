using Microsoft.EntityFrameworkCore;
using QuickNote_Data.Entities;

namespace QuickNote_Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
            
        public DbSet<UserEntity> Users { get; set; }
    }
}