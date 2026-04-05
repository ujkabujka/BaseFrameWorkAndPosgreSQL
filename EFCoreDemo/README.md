# EFCoreDemo

`EFCoreDemo` is a console-based learning project for PostgreSQL, Npgsql, and Entity Framework Core. The goal is to move from basic database communication to more practical EF Core features in small, runnable examples.

## Learning path

1. Configure a connection string.
2. Understand entity classes and `DbContext`.
3. Create tables with EF Core migrations.
4. Insert, read, update, and delete rows.
5. Model relationships with a join table.
6. Use LINQ with `Include`, projections, and filtering.
7. Run raw SQL when EF Core is not enough.
8. Save JSON into PostgreSQL `jsonb`.
9. Wrap multiple changes in a transaction.

## Project structure

- `Configuration/ConnectionStringResolver.cs`
  Resolves the connection string from environment variables or `appsettings.local.json`.
- `Configuration/AppDbContextFactory.cs`
  Lets `dotnet ef` create `AppDbContext` at design time.
- `Data/AppDbContext.cs`
  Holds table mappings, relationships, indexes, precision, enum conversion, and JSON configuration.
- `Models/`
  Contains the entity classes used throughout the examples.
- `Examples/StudyRunner.cs`
  Contains the runnable examples for each topic.

## Domain model used for learning

- `Student`
  Basic table with a unique email column.
- `Course`
  Demonstrates regular scalar properties and a typed JSON document (`Details`).
- `Enrollment`
  Demonstrates a many-to-many relationship with extra columns such as `ProgressPercent`.
- `AuditLog`
  Demonstrates raw JSON string storage using PostgreSQL `jsonb`.

## Prerequisites

- .NET SDK 8
- PostgreSQL server, local or remote
- `dotnet ef` tool

Check the EF CLI:

```bash
dotnet ef --version
```

## Step 1: Start PostgreSQL locally

Docker is the easiest local setup:

```bash
docker run --name pg-study ^
  -e POSTGRES_PASSWORD=devpassword ^
  -e POSTGRES_DB=efcore_study ^
  -p 5432:5432 ^
  -d postgres:16
```

If you want to use Supabase instead, use its PostgreSQL connection string and place it in one of the supported sources below.

## Step 2: Set the connection string

Priority order:

1. `EFCOREDEMO_CONNECTION_STRING`
2. `PG_CONNECTION_STRING`
3. `EFCoreDemo/appsettings.local.json`
4. `EFCoreDemo/appsettings.example.json`
5. fallback local default: `Host=localhost;Port=5432;Database=efcore_study;Username=postgres;Password=devpassword`

Example `EFCoreDemo/appsettings.example.json` or `EFCoreDemo/appsettings.local.json`:

```json
{
  "ConnectionString": "Host=localhost;Port=5432;Database=efcore_study;Username=postgres;Password=devpassword"
}
```

PowerShell environment variable example:

```powershell
$env:PG_CONNECTION_STRING="Host=aws-...supabase.com;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
```

## Step 3: Build and inspect commands

```bash
dotnet build BaseFrameWorkAndPosgreSQL.sln
dotnet run --project EFCoreDemo -- help
dotnet run --project EFCoreDemo -- info
dotnet run --project EFCoreDemo -- canconnect
```

## VS Code debugging

This project now uses synchronous code paths so stepping through the learning flow is simpler in the debugger.

Use these launch configurations in VS Code:

- `EFCoreDemo: Debug Command`
  Prompts you for a study command such as `basic`, `json`, or `all`.
- `EFCoreDemo: Debug Help`
  Starts the app with the `help` command.
- `EFCoreDemo: Debug All`
  Starts the full walkthrough.

Debug steps:

1. Open the repository in VS Code.
2. Put breakpoints anywhere in `Program.cs`, `Examples/StudyRunner.cs`, or `Data/AppDbContext.cs`.
3. Press `F5`.
4. Choose one of the `EFCoreDemo` launch profiles.

If you use Supabase, set `PG_CONNECTION_STRING` in the terminal or create `EFCoreDemo/appsettings.local.json` before debugging.

## Step 4: Create tables with migrations

This project uses EF Core migrations, which is the correct approach for real applications.

Create a migration:

```bash
dotnet ef migrations add InitialLearningSchema --project EFCoreDemo --startup-project EFCoreDemo
```

Apply migrations to the database:

```bash
dotnet ef database update --project EFCoreDemo --startup-project EFCoreDemo
```

Or use the app command:

```bash
dotnet run --project EFCoreDemo -- migrate
```

What to learn here:

- entity class changes affect the model
- migrations capture schema changes
- `database update` applies those changes to PostgreSQL
- `AppDbContextFactory` is what makes design-time migrations work

## Step 5: Insert seed rows

```bash
dotnet run --project EFCoreDemo -- seed
```

This inserts:

- 3 students
- 3 courses
- 4 enrollments
- 1 audit log row

Study points:

- `Add` and `AddRange`
- `SaveChangesAsync`
- generated primary keys
- inserting related rows after parent rows exist

## Step 6: Basic CRUD example

```bash
dotnet run --project EFCoreDemo -- basic
```

This command shows:

- `INSERT` with `db.Students.Add(...)`
- `SELECT` with sorting and filtering
- `AsNoTracking()` for read-only queries
- `UPDATE` by changing a tracked entity
- `DELETE` by removing the entity and saving again

## Step 7: Relationship example

```bash
dotnet run --project EFCoreDemo -- relationships
```

This command shows:

- many-to-many modeling with an explicit join entity
- `Include` and `ThenInclude`
- projection with `Select`
- aggregate values such as `Count` and `Average`

## Step 8: JSON storage example

```bash
dotnet run --project EFCoreDemo -- json
```

This project intentionally shows two JSON styles:

- typed JSON on `Course.Details`
  This uses EF Core JSON mapping with `ToJson()` and stores a structured object inside one `jsonb` column.
- raw JSON string on `AuditLog.Payload`
  This stores a JSON string in a `jsonb` column when you want full manual control of the payload.

Study points:

- PostgreSQL `jsonb`
- serializing with `System.Text.Json`
- querying typed JSON data with LINQ

## Step 9: Raw SQL example

```bash
dotnet run --project EFCoreDemo -- rawsql
```

This command shows:

- `FromSqlInterpolated(...)`
- parameterized SQL
- a PostgreSQL JSON operator query: `payload ->> 'area'`

## Step 10: Transaction example

```bash
dotnet run --project EFCoreDemo -- transaction
```

This command wraps two inserts in the same transaction:

- a `Student` row
- an `AuditLog` row

Study points:

- `BeginTransactionAsync`
- commit behavior
- grouping multiple database changes into one unit of work

## Run the full walkthrough

```bash
dotnet run --project EFCoreDemo -- all
```

## Code examples by concept

Create a `DbSet`:

```csharp
public DbSet<Student> Students => Set<Student>();
```

Create a table mapping:

```csharp
modelBuilder.Entity<Student>(entity =>
{
    entity.ToTable("students");
    entity.HasKey(student => student.Id);
    entity.Property(student => student.FullName).IsRequired().HasMaxLength(120);
});
```

Insert a row:

```csharp
db.Students.Add(new Student
{
    FullName = "New Student",
    Email = "new@student.local"
});

await db.SaveChangesAsync();
```

Read rows:

```csharp
var students = await db.Students
    .AsNoTracking()
    .OrderBy(student => student.FullName)
    .ToListAsync();
```

Update a row:

```csharp
student.FullName = "Updated Name";
await db.SaveChangesAsync();
```

Delete a row:

```csharp
db.Students.Remove(student);
await db.SaveChangesAsync();
```

JSON storage:

```csharp
entity.OwnsOne(course => course.Details, details =>
{
    details.ToJson();
    details.OwnsMany(detail => detail.Modules);
});
```

## Important notes

- The previous hardcoded database password was removed from the code. Keep secrets in environment variables or `appsettings.local.json`.
- SQL logging is enabled in `AppDbContext` because this is a study project. That is useful for learning, but you would usually reduce logging in production.
- For real projects, prefer migrations over `EnsureCreated()`.

## Suggested next exercises

- add `CreatedBy` and `UpdatedBy` columns
- create a repository or service layer
- add pagination examples
- add optimistic concurrency examples
- connect the data layer to your BaseFramework UI project later
