# BaseFrameWorkAndPosgreSQL

This repository is now organized as a PostgreSQL + Entity Framework Core study project.

The main learning project is `EFCoreDemo/EFCoreDemo.csproj`. It demonstrates:

- connection setup for local PostgreSQL or Supabase
- entity and `DbContext` configuration
- EF Core migrations
- EF Core database-first scaffolding
- basic CRUD operations
- relationships with a join table
- raw SQL examples
- PostgreSQL `jsonb` storage with both typed and raw JSON examples
- transactions

Quick start:

```bash
dotnet build BaseFrameWorkAndPosgreSQL.sln
dotnet run --project EFCoreDemo -- help
```

For the full walkthrough, read `EFCoreDemo/README.md`.
