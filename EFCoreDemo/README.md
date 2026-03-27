# EFCoreDemo

A basic .NET console application demonstrating Entity Framework Core with PostgreSQL.

## Prerequisites

- .NET 10.0 SDK
- Docker (for PostgreSQL)
- PostgreSQL running in Docker container

## Setup

1. Ensure PostgreSQL is running:
   ```bash
   docker run -d --name postgres-dev -e POSTGRES_PASSWORD=devpassword -e POSTGRES_DB=devdb -p 5432:5432 postgres:15
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## What this demo does

This application demonstrates basic database operations using Entity Framework Core:

1. **Database Creation**: Uses `EnsureCreated()` to create the database and tables if they don't exist.
2. **Data Seeding**: Adds sample data (5 people with names, heights, and ages).
3. **Data Retrieval**: Queries and displays all records.
4. **Data Filtering**: Shows how to filter data using LINQ.

## Database Schema

The `Deneme` table has:
- `Name` (string, primary key)
- `Height` (double)
- `Age` (int)

## Entity Framework Core Basics

### DbContext
- `AppDbContext` inherits from `DbContext`
- Represents a session with the database
- Configured in `OnConfiguring` method with connection string

### Models
- `Deneme` class represents the database table
- Properties map to columns
- `[Key]` attribute marks the primary key

### Operations
- `AddRange()`: Adds multiple entities
- `SaveChanges()`: Commits changes to database
- `ToList()`: Executes query and returns results
- `Where()`: Filters data with LINQ

## Migrations

Migrations allow versioning database schema changes:

1. Create migration: `dotnet ef migrations add MigrationName`
2. Apply migration: `dotnet ef database update`
3. View migrations: `dotnet ef migrations list`

In this demo, we used `EnsureCreated()` for simplicity, but migrations are preferred for production.

## Connection String

```
Host=localhost;Database=devdb;Username=postgres;Password=devpassword
```

## Next Steps

- Learn about authentication and security
- Implement CRUD operations (Create, Read, Update, Delete)
- Use migrations for schema changes
- Add relationships between tables