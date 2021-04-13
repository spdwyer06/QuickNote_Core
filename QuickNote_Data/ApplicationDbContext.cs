using Microsoft.EntityFrameworkCore;
using QuickNote_Data.Entities;

namespace QuickNote_Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
            
        public DbSet<UserEntity> Users { get; set; }

        public DbSet<NoteEntity> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Creating a many-to-one relationship between Note and User
            modelBuilder.Entity<NoteEntity>()
                // Note has one User
                .HasOne(n => n.Owner)
                // An User can have many Notes
                .WithMany(p => p.Notes)
                // Specifying the OwnerId property as the FK
                .HasForeignKey(n => n.OwnerId);
        }
    }
}