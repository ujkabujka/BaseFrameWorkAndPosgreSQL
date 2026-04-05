using EFCoreDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCoreDemo.Data;

public class AppDbContext(string connectionString) : DbContext
{
    private readonly string _connectionString = connectionString;

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");
            entity.HasKey(student => student.Id);
            entity.Property(student => student.FullName).IsRequired().HasMaxLength(120);
            entity.Property(student => student.Email).IsRequired().HasMaxLength(150);
            entity.Property(student => student.RegisteredAtUtc);
            entity.HasIndex(student => student.Email).IsUnique();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses");
            entity.HasKey(course => course.Id);
            entity.Property(course => course.Title).IsRequired().HasMaxLength(150);
            entity.Property(course => course.Price).HasPrecision(10, 2);
            entity.Property(course => course.Difficulty).HasConversion<string>().HasMaxLength(20);
            entity.Property(course => course.CreatedAtUtc);
            entity.HasIndex(course => course.Title).IsUnique();

            entity.OwnsOne(course => course.Details, details =>
            {
                details.ToJson();
                details.Property(detail => detail.Summary).HasMaxLength(400);
                details.OwnsMany(detail => detail.Modules, module =>
                {
                    module.Property(item => item.Title).HasMaxLength(120);
                });
            });
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("enrollments");
            entity.HasKey(enrollment => new { enrollment.StudentId, enrollment.CourseId });
            entity.Property(enrollment => enrollment.ProgressPercent);
            entity.Property(enrollment => enrollment.EnrolledAtUtc);

            entity
                .HasOne(enrollment => enrollment.Student)
                .WithMany(student => student.Enrollments)
                .HasForeignKey(enrollment => enrollment.StudentId);

            entity
                .HasOne(enrollment => enrollment.Course)
                .WithMany(course => course.Enrollments)
                .HasForeignKey(enrollment => enrollment.CourseId);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.EventName).IsRequired().HasMaxLength(100);
            entity.Property(log => log.Payload).IsRequired().HasColumnType("jsonb");
            entity.Property(log => log.CreatedAtUtc);
            entity.HasIndex(log => log.CreatedAtUtc);
        });
    }
}
