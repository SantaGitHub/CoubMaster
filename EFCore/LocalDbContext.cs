using JewishCat.DiscordBot.Models;
using Microsoft.EntityFrameworkCore;

namespace JewishCat.DiscordBot.EFCore
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(k => k.Id);
                entity.HasData(new User(1, "test"));
            });
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=CustomerDB.db;");
        }
    }
}