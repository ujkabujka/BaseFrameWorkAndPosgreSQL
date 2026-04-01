using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo;

public class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Deneme> Denemes => Set<Deneme>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}
