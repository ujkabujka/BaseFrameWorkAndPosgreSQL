# EFCoreDemo (PostgreSQL + C#/.NET Basics)

A small console app to study PostgreSQL connection and data operations with EF Core.

## What was improved

- Connection string is now configurable through environment variable (`PG_CONNECTION_STRING`).
- App performs a **connection test first**, then gives actionable help if PostgreSQL is not reachable.
- Output is structured for easier debugging/study sessions.

## Prerequisites

- .NET SDK (project targets `net10.0`)
- A reachable PostgreSQL server (local or cloud)

## Option A — Start PostgreSQL locally with Docker (recommended)

```bash
docker run --name pg-study \
  -e POSTGRES_PASSWORD=devpassword \
  -e POSTGRES_DB=devdb \
  -p 5432:5432 \
  -d postgres:16
```

Then run:

```bash
dotnet run --project EFCoreDemo
```

## Option B — Use a free managed PostgreSQL instance

You can use providers like Neon or Supabase and paste the provided connection string:

```bash
export PG_CONNECTION_STRING="Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
dotnet run --project EFCoreDemo
```

## What the app demonstrates

1. Connection check via `NpgsqlConnection`.
2. Database/table creation via `EnsureCreated()`.
3. Seed data insertion.
4. Query all rows and filtered rows using LINQ.

## Useful study/debug tips

- Set `PG_CONNECTION_STRING` per terminal session to switch environments quickly.
- Keep `EnsureCreated()` for learning; move to EF Core Migrations for real projects.
- To see SQL statements, add EF Core logging in `AppDbContext` later as your next exercise.
