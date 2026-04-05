using System.Text.Json;
using EFCoreDemo.Configuration;
using EFCoreDemo.Data;
using EFCoreDemo.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EFCoreDemo.Examples;

public sealed class StudyRunner(string connectionString)
{
    private readonly string _connectionString = connectionString;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

    public int PrintHelp()
    {
        PrintSection("EF Core PostgreSQL Study Runner");
        Console.WriteLine("Commands:");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- help");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- info");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- canconnect");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- migrate");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- seed");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- basic");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- queries");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- relationships");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- json");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- rawsql");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- transaction");
        Console.WriteLine("  dotnet run --project EFCoreDemo -- all");
        Console.WriteLine();
        Console.WriteLine("Migration commands:");
        Console.WriteLine("  dotnet ef migrations add AddSomethingNew --project EFCoreDemo --startup-project EFCoreDemo");
        Console.WriteLine("  dotnet ef database update --project EFCoreDemo --startup-project EFCoreDemo");
        Console.WriteLine("  dotnet ef migrations remove --project EFCoreDemo --startup-project EFCoreDemo");
        Console.WriteLine();
        Console.WriteLine("Connection string priority:");
        Console.WriteLine("  1. EFCOREDEMO_CONNECTION_STRING");
        Console.WriteLine("  2. PG_CONNECTION_STRING");
        Console.WriteLine("  3. EFCoreDemo/appsettings.local.json");
        Console.WriteLine("  4. EFCoreDemo/appsettings.example.json");
        Console.WriteLine("  5. Local default: Host=localhost;Port=5432;Database=efcore_study;Username=postgres;Password=devpassword");
        Console.WriteLine();
        Console.WriteLine("Database-first scaffolding study:");
        Console.WriteLine("  - script: .\\scripts\\Scaffold-DatabaseFirst.ps1");
        Console.WriteLine("  - guide : EFCoreDemo/Scaffolding/DatabaseFirst/README.md");
        return 0;
    }

    public int PrintUnknownCommand(string command)
    {
        Console.WriteLine($"Unknown command: {command}");
        Console.WriteLine("Run `dotnet run --project EFCoreDemo -- help` to see the study commands.");
        return PrintHelp();
    }

    public int PrintConnectionInfo()
    {
        PrintSection("Connection Info");
        Console.WriteLine(ConnectionStringResolver.Describe(_connectionString));
        return 0;
    }

    public int CanConnect()
    {
        PrintSection("Connection Test");

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            Console.WriteLine($"Connected to PostgreSQL {connection.PostgreSqlVersion}.");
            Console.WriteLine(ConnectionStringResolver.Describe(_connectionString));
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Connection failed.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("Set PG_CONNECTION_STRING, EFCoreDemo/appsettings.local.json, or EFCoreDemo/appsettings.example.json, then run the command again.");
            return 1;
        }
    }

    public int ApplyMigrations()
    {
        PrintSection("Apply Migrations");

        using var db = CreateDbContext();
        var pendingMigrations = db.Database.GetPendingMigrations().ToList();

        if (!pendingMigrations.Any())
        {
            Console.WriteLine("No pending migrations were found.");
            return 0;
        }

        Console.WriteLine("Pending migrations:");
        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine($"  - {migration}");
        }

        db.Database.Migrate();
        Console.WriteLine("Database schema is now up to date.");
        return 0;
    }

    public int SeedSampleData()
    {
        PrintSection("Seed Sample Data");

        using var db = CreateDbContext();
        db.Database.Migrate();

        if (db.Students.Any() || db.Courses.Any())
        {
            Console.WriteLine("Sample data already exists, so the seed step was skipped.");
            return 0;
        }

        var students = new[]
        {
            new Student { FullName = "Ada Lovelace", Email = "ada@study.local" },
            new Student { FullName = "Alan Turing", Email = "alan@study.local" },
            new Student { FullName = "Grace Hopper", Email = "grace@study.local" }
        };

        var courses = new[]
        {
            new Course
            {
                Title = "EF Core Basics",
                Price = 49.90m,
                Difficulty = CourseDifficulty.Beginner,
                IsPublished = true,
                Details = new CourseDetails
                {
                    Summary = "Create tables, add rows, query rows, and understand DbContext.",
                    EstimatedHours = 6,
                    Modules =
                    {
                        new CourseModule { Title = "DbContext and DbSet", DurationMinutes = 60 },
                        new CourseModule { Title = "Basic CRUD", DurationMinutes = 80 }
                    }
                }
            },
            new Course
            {
                Title = "PostgreSQL Relationships",
                Price = 79.00m,
                Difficulty = CourseDifficulty.Intermediate,
                IsPublished = true,
                Details = new CourseDetails
                {
                    Summary = "One-to-many and many-to-many examples with LINQ queries.",
                    EstimatedHours = 8,
                    Modules =
                    {
                        new CourseModule { Title = "Foreign Keys", DurationMinutes = 70 },
                        new CourseModule { Title = "Include and ThenInclude", DurationMinutes = 75 }
                    }
                }
            },
            new Course
            {
                Title = "JSON Storage in PostgreSQL",
                Price = 99.00m,
                Difficulty = CourseDifficulty.Advanced,
                IsPublished = true,
                Details = new CourseDetails
                {
                    Summary = "Store structured data in jsonb and query it from EF Core.",
                    EstimatedHours = 10,
                    Modules =
                    {
                        new CourseModule { Title = "jsonb Basics", DurationMinutes = 65 },
                        new CourseModule { Title = "Typed JSON Mapping", DurationMinutes = 90 }
                    }
                }
            }
        };

        db.Students.AddRange(students);
        db.Courses.AddRange(courses);
        db.SaveChanges();

        db.Enrollments.AddRange(
            new Enrollment { StudentId = students[0].Id, CourseId = courses[0].Id, ProgressPercent = 35 },
            new Enrollment { StudentId = students[0].Id, CourseId = courses[1].Id, ProgressPercent = 10 },
            new Enrollment { StudentId = students[1].Id, CourseId = courses[0].Id, ProgressPercent = 90 },
            new Enrollment { StudentId = students[2].Id, CourseId = courses[2].Id, ProgressPercent = 50 });

        db.AuditLogs.Add(new AuditLog
        {
            EventName = "seed.completed",
            Payload = JsonSerializer.Serialize(new
            {
                area = "seed",
                studentCount = students.Length,
                courseCount = courses.Length,
                generatedAtUtc = DateTime.UtcNow
            }, _jsonOptions)
        });

        db.SaveChanges();

        Console.WriteLine("Inserted 3 students, 3 courses, 4 enrollments, and 1 audit log.");
        return 0;
    }

    public int RunBasicCrud()
    {
        PrintSection("Basic CRUD");

        using var db = CreateDbContext();
        db.Database.Migrate();

        var tempStudent = new Student
        {
            FullName = "Basic CRUD Demo",
            Email = $"basic-{DateTime.UtcNow:yyyyMMddHHmmssfff}@study.local"
        };

        db.Students.Add(tempStudent);
        db.SaveChanges();
        Console.WriteLine($"Inserted student with Id={tempStudent.Id}.");

        // WHERE + ORDER BY + LIMIT
        // EF Core translates this into a SELECT query with filtering/sorting in PostgreSQL.
        // AsNoTracking() is useful for read-only queries because EF does not track entity changes.
        var firstStudents = db.Students
            .AsNoTracking()
            .OrderBy(student => student.FullName)
            .Take(5)
            .ToList();

        Console.WriteLine("Read example:");
        foreach (var student in firstStudents)
        {
            Console.WriteLine($"  {student.Id} | {student.FullName} | {student.Email}");
        }

        // WHERE Email LIKE '%@study.local' ORDER BY Email
        // This is a basic filtering example. EndsWith() becomes a SQL string filter.
        var filteredStudents = db.Students
            .AsNoTracking()
            .Where(student => student.Email.EndsWith("@study.local"))
            .OrderBy(student => student.Email)
            .ToList();

        Console.WriteLine($"Filter example returned {filteredStudents.Count} study records.");

        tempStudent.FullName = "Basic CRUD Demo Updated";
        db.SaveChanges();
        Console.WriteLine("Updated the inserted student.");

        db.Students.Remove(tempStudent);
        db.SaveChanges();
        Console.WriteLine("Deleted the temporary student.");

        return 0;
    }

    public int RunQueryLearningExamples()
    {
        PrintSection("Query Learning Examples");

        using var db = CreateDbContext();
        db.Database.Migrate();
        SeedIfEmpty(db);

        // COUNT(*)
        // EF Core translates Count() to SQL COUNT on the database side.
        var totalStudents = db.Students.Count();
        Console.WriteLine($"Count example: total students = {totalStudents}");

        // EXISTS(...)
        // Any() is usually translated as an efficient EXISTS query.
        var hasAdvancedCourse = db.Courses.Any(course => course.Difficulty == CourseDifficulty.Advanced);
        Console.WriteLine($"Any example: advanced course exists = {hasAdvancedCourse}");

        // SELECT TOP 1 / LIMIT 1
        // FirstOrDefault() returns one row or null/default if nothing matches.
        var beginnerCourse = db.Courses
            .AsNoTracking()
            .FirstOrDefault(course => course.Difficulty == CourseDifficulty.Beginner);
        Console.WriteLine($"FirstOrDefault example: beginner course = {beginnerCourse?.Title ?? "not found"}");

        // INNER JOIN students -> enrollments -> courses
        // This explicit LINQ join is close to SQL JOIN syntax and is useful for learning.
        var joinRows = (
            from student in db.Students.AsNoTracking()
            join enrollment in db.Enrollments.AsNoTracking() on student.Id equals enrollment.StudentId
            join course in db.Courses.AsNoTracking() on enrollment.CourseId equals course.Id
            orderby student.FullName, course.Title
            select new
            {
                StudentName = student.FullName,
                CourseTitle = course.Title,
                enrollment.ProgressPercent
            })
            .ToList();

        Console.WriteLine("Join example:");
        foreach (var row in joinRows)
        {
            Console.WriteLine($"  {row.StudentName} -> {row.CourseTitle} ({row.ProgressPercent}%)");
        }

        // GROUP BY with COUNT and AVG
        // This is the EF Core equivalent of:
        // SELECT CourseId, COUNT(*), AVG(ProgressPercent) FROM enrollments GROUP BY CourseId
        var groupedProgress = db.Enrollments
            .AsNoTracking()
            .GroupBy(enrollment => enrollment.CourseId)
            .Select(group => new
            {
                CourseId = group.Key,
                StudentCount = group.Count(),
                AverageProgress = Math.Round(group.Average(item => item.ProgressPercent), 1)
            })
            .OrderBy(item => item.CourseId)
            .ToList();

        Console.WriteLine("GroupBy example:");
        foreach (var group in groupedProgress)
        {
            Console.WriteLine($"  CourseId={group.CourseId} | count={group.StudentCount} | avg={group.AverageProgress}");
        }

        // LEFT JOIN behavior
        // GroupJoin + DefaultIfEmpty() is the usual LINQ pattern for a SQL LEFT JOIN.
        var leftJoinRows = (
            from student in db.Students.AsNoTracking()
            join enrollment in db.Enrollments.AsNoTracking() on student.Id equals enrollment.StudentId into studentEnrollments
            from enrollment in studentEnrollments.DefaultIfEmpty()
            select new
            {
                student.FullName,
                CourseId = enrollment != null ? enrollment.CourseId : (int?)null
            })
            .OrderBy(item => item.FullName)
            .ToList();

        Console.WriteLine("Left join example:");
        foreach (var row in leftJoinRows.Take(5))
        {
            Console.WriteLine($"  {row.FullName} | CourseId={row.CourseId?.ToString() ?? "NULL"}");
        }

        // Pagination
        // Skip/Take is translated to OFFSET/LIMIT in PostgreSQL.
        var pageNumber = 1;
        var pageSize = 2;
        var pagedStudents = db.Students
            .AsNoTracking()
            .OrderBy(student => student.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToList();

        Console.WriteLine("Pagination example:");
        foreach (var student in pagedStudents)
        {
            Console.WriteLine($"  page row -> {student.Id} | {student.FullName}");
        }

        return 0;
    }

    public int RunRelationshipExamples()
    {
        PrintSection("Relationships");

        using var db = CreateDbContext();
        db.Database.Migrate();
        SeedIfEmpty(db);

        // Include + ThenInclude does not mean "manual join result rows" in your C# code.
        // It tells EF Core to load related entities as an object graph:
        // Student -> Enrollments -> Course
        // In SQL terms this is fulfilled by related queries or joins generated by EF Core.
        var student = db.Students
            .Include(item => item.Enrollments)
            .ThenInclude(item => item.Course)
            .OrderBy(item => item.Id)
            .First();

        Console.WriteLine($"{student.FullName} is enrolled in {student.Enrollments.Count} course(s).");
        foreach (var enrollment in student.Enrollments)
        {
            Console.WriteLine($"  {enrollment.Course.Title} | progress {enrollment.ProgressPercent}%");
        }

        var missingCourse = db.Courses
            .OrderBy(course => course.Id)
            .FirstOrDefault(course => student.Enrollments.All(enrollment => enrollment.CourseId != course.Id));

        if (missingCourse is not null)
        {
            db.Enrollments.Add(new Enrollment
            {
                StudentId = student.Id,
                CourseId = missingCourse.Id,
                ProgressPercent = 0
            });
            db.SaveChanges();
            Console.WriteLine($"Added new enrollment: {student.FullName} -> {missingCourse.Title}");
        }

        // Projection example:
        // Instead of loading full entity graphs, Select() shapes only the data we want.
        // This is similar to writing a custom SELECT list in SQL.
        var courseSummaries = db.Courses
            .AsNoTracking()
            .OrderBy(course => course.Title)
            .Select(course => new
            {
                course.Title,
                StudentCount = course.Enrollments.Count,
                AverageProgress = course.Enrollments.Any()
                    ? Math.Round(course.Enrollments.Average(enrollment => enrollment.ProgressPercent), 1)
                    : 0
            })
            .ToList();

        Console.WriteLine("Projection example:");
        foreach (var summary in courseSummaries)
        {
            Console.WriteLine($"  {summary.Title} | students={summary.StudentCount} | avg progress={summary.AverageProgress}");
        }

        return 0;
    }

    public int RunJsonExamples()
    {
        PrintSection("JSON Storage");

        using var db = CreateDbContext();
        db.Database.Migrate();
        SeedIfEmpty(db);

        // Filtering by values inside a JSON-mapped owned type.
        // EF Core + Npgsql translates this into PostgreSQL jsonb access under the hood.
        var advancedCourses = db.Courses
            .AsNoTracking()
            .Where(course => course.Details.EstimatedHours >= 8)
            .OrderBy(course => course.Details.EstimatedHours)
            .ToList();

        Console.WriteLine("Typed jsonb example with ToJson():");
        foreach (var course in advancedCourses)
        {
            Console.WriteLine($"  {course.Title} | estimated hours={course.Details.EstimatedHours}");
            foreach (var module in course.Details.Modules)
            {
                Console.WriteLine($"    module: {module.Title} ({module.DurationMinutes} min)");
            }
        }

        var jsonAuditPayload = JsonSerializer.Serialize(new
        {
            area = "json-demo",
            action = "read-and-write",
            tags = new[] { "jsonb", "ef-core", "postgresql" },
            createdAtUtc = DateTime.UtcNow
        }, _jsonOptions);

        db.AuditLogs.Add(new AuditLog
        {
            EventName = "json.example.saved",
            Payload = jsonAuditPayload
        });
        db.SaveChanges();

        Console.WriteLine("Saved a raw JSON string into the audit_logs.payload jsonb column.");
        Console.WriteLine(jsonAuditPayload);
        return 0;
    }

    public int RunRawSqlExamples()
    {
        PrintSection("Raw SQL");

        using var db = CreateDbContext();
        db.Database.Migrate();
        SeedIfEmpty(db);

        const decimal minimumPrice = 70m;

        // Raw SQL example.
        // Use this when EF LINQ is not enough or when you want exact SQL control.
        // FromSqlInterpolated keeps the query parameterized, which helps protect against SQL injection.
        var expensiveCourses = db.Courses
            .FromSqlInterpolated($@"SELECT * FROM courses WHERE ""Price"" >= {minimumPrice}")
            .AsNoTracking()
            .OrderBy(course => course.Price)
            .ToList();

        Console.WriteLine("FromSqlInterpolated example:");
        foreach (var course in expensiveCourses)
        {
            Console.WriteLine($"  {course.Title} | {course.Price:C}");
        }

        var jsonArea = "json-demo";
        // PostgreSQL-specific JSON operator:
        // payload ->> 'area'
        // This reads the 'area' property from a jsonb column as text.
        var matchingAuditLogs = db.AuditLogs
            .FromSqlInterpolated($@"SELECT * FROM audit_logs WHERE payload ->> 'area' = {jsonArea}")
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAtUtc)
            .Take(5)
            .ToList();

        Console.WriteLine($"PostgreSQL JSON operator example returned {matchingAuditLogs.Count} row(s).");
        return 0;
    }

    public int RunTransactionExample()
    {
        PrintSection("Transaction");

        using var db = CreateDbContext();
        db.Database.Migrate();

        using var transaction = db.Database.BeginTransaction();

        var tempStudent = new Student
        {
            FullName = "Transaction Demo",
            Email = $"transaction-{DateTime.UtcNow:yyyyMMddHHmmssfff}@study.local"
        };

        db.Students.Add(tempStudent);
        db.SaveChanges();

        db.AuditLogs.Add(new AuditLog
        {
            EventName = "transaction.example.committed",
            Payload = JsonSerializer.Serialize(new
            {
                area = "transaction",
                studentEmail = tempStudent.Email,
                committedAtUtc = DateTime.UtcNow
            }, _jsonOptions)
        });
        db.SaveChanges();

        transaction.Commit();

        Console.WriteLine("Inserted one student and one audit log inside a single transaction.");
        return 0;
    }

    public int RunFullStudyGuide()
    {
        PrintConnectionInfo();
        CanConnect();
        ApplyMigrations();
        SeedSampleData();
        RunBasicCrud();
        RunQueryLearningExamples();
        RunRelationshipExamples();
        RunJsonExamples();
        RunRawSqlExamples();
        RunTransactionExample();
        return 0;
    }

    private AppDbContext CreateDbContext() => new(_connectionString);

    private void SeedIfEmpty(AppDbContext db)
    {
        if (!db.Students.Any() || !db.Courses.Any())
        {
            SeedSampleData();
        }
    }

    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', title.Length));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', title.Length));
    }
}
