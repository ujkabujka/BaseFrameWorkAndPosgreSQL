using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo
{
    // DbContext is the main class in Entity Framework Core that coordinates
    // Entity Framework functionality for a given data model.
    // It represents a session with the database and allows querying and saving data.
    public class AppDbContext : DbContext
    {
        // DbSet represents a table in the database.
        // This will create a "Denemes" table (pluralized by convention).
        public DbSet<Deneme> Denemes { get; set; }

        // This method is called when configuring the database connection.
        // Here we specify we're using PostgreSQL with Npgsql provider.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connection string for PostgreSQL
            // Host: localhost (since our Docker container exposes port 5432)
            // Database: devdb (the database we created in the container)
            // Username/Password: as set in the Docker run command
            optionsBuilder.UseNpgsql("Host=localhost;Database=devdb;Username=postgres;Password=devpassword");
        }
    }
}