using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo;

public class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");

        modelBuilder.Entity<User>().HasKey(u => u.Id);

        modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("id");
        modelBuilder.Entity<User>().Property(u => u.Username).HasColumnName("user_name");
        modelBuilder.Entity<User>().Property(u => u.Email).HasColumnName("email");
    }
}
