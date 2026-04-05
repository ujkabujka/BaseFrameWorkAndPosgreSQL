# Database-First Scaffolding Study

This folder is for the second EF Core workflow:

- `Code First`
  You change C# classes and use migrations to update the database.
- `Database First`
  You start from an existing database and let EF Core generate the `DbContext` and model classes for you.

This second workflow is called reverse engineering or scaffolding.

According to the official EF Core documentation, scaffolding reads database schema information such as tables, columns, constraints, indexes, and foreign keys, and then generates a `DbContext` plus entity classes from that schema.

## Why this matters for your project

You said you want two ways of working:

1. Known database design
   You create or edit the C# models and push those changes to PostgreSQL.
2. Unknown or existing database
   You inspect the existing database and pull its structure into generated C# models.

That is exactly the difference between code-first and database-first.

## Basic study path

### Level 1: Scaffold everything from an existing database

This is the easiest way to learn database-first:

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1
```

What it does:

- reads the connection string from env vars or appsettings files
- connects to PostgreSQL
- reads the database schema
- generates a `DbContext`
- generates entity classes for tables/views it can map

Default output:

- context: `EFCoreDemo/Scaffolding/DatabaseFirst/Context/DatabaseFirstContext.cs`
- models: `EFCoreDemo/Scaffolding/DatabaseFirst/Models/Generated/*.cs`

### Level 2: Scaffold only selected tables

This is useful when the database is large:

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1 -Tables students,courses,enrollments -ContextName SchoolCatalogContext -Force
```

Study points:

- `--table` / `-Tables`
- smaller generated model
- faster iteration

### Level 3: Scaffold only one schema

PostgreSQL often uses the `public` schema:

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1 -Schemas public -Force
```

### Level 4: Preserve the original database names

Normally EF Core changes names to look more like C# naming conventions.

If you want database names exactly as they exist:

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1 -UseDatabaseNames -Force
```

Study points:

- database naming vs C# naming
- tradeoff between readability and exact schema matching

### Level 5: Use attributes where possible

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1 -DataAnnotations -Force
```

Study points:

- generated attributes like `[Key]`, `[StringLength]`
- Fluent API is still used for things attributes cannot express

### Level 6: Re-scaffold safely

If the database changes later, you can regenerate the classes:

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1 -Force
```

Important rule:

- do not edit generated files directly if you plan to re-scaffold later
- extend them using partial classes or partial `DbContext`

## Advanced study path

### Advanced 1: Keep generated code and custom code separate

Use this pattern:

- generated files go in `Models/Generated`
- your custom files go in `Models/Custom`
- generated context goes in `Context`
- your extra configuration goes in `Customizations`

### Advanced 2: Extend generated entities with partial classes

See:

- `Examples/ScaffoldedEntity.Partial.cs.example`
- `Examples/ScaffoldedDbContext.Partial.cs.example`

This is the safe way to add behavior without losing it during `-Force` re-scaffolding.

### Advanced 3: Switch from database-first to code-first later

One valid learning path is:

1. Scaffold from an existing database.
2. Study the generated model and relationships.
3. Clean up or reorganize the generated code.
4. Decide that C# becomes the source of truth.
5. Start using EF Core migrations for future schema changes.

This is supported by the official EF Core guidance. That guidance also notes that repeated scaffolding overwrites generated code, so custom code should live in partial files or separate configuration.

## Important limitations

EF Core scaffolding is powerful, but it cannot infer everything from a database schema.

From the official documentation:

- inheritance hierarchies cannot be inferred from schema alone
- owned types cannot be inferred from schema alone
- table splitting cannot be inferred from schema alone
- unsupported provider-specific column types may not scaffold

That means:

- database-first is excellent for tables, columns, keys, and relationships
- code-first is better when your model includes richer domain concepts

## Raw CLI examples

These are the direct EF Core commands behind the script.

Scaffold all tables:

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=efcore_study;Username=postgres;Password=devpassword" Npgsql.EntityFrameworkCore.PostgreSQL --project EFCoreDemo --startup-project EFCoreDemo --context DatabaseFirstContext --context-dir Scaffolding/DatabaseFirst/Context --output-dir Scaffolding/DatabaseFirst/Models/Generated --namespace EFCoreDemo.Scaffolding.DatabaseFirst.Models.Generated --context-namespace EFCoreDemo.Scaffolding.DatabaseFirst.Context --no-onconfiguring
```

Scaffold selected tables only:

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=efcore_study;Username=postgres;Password=devpassword" Npgsql.EntityFrameworkCore.PostgreSQL --project EFCoreDemo --startup-project EFCoreDemo --context SchoolCatalogContext --context-dir Scaffolding/DatabaseFirst/Context --output-dir Scaffolding/DatabaseFirst/Models/Generated --table students --table courses --table enrollments --force --no-onconfiguring
```

## Suggested exercises

1. Scaffold your current database into `DatabaseFirstContext`.
2. Compare the generated classes with your code-first entities.
3. Add one new table directly in PostgreSQL.
4. Run scaffolding again with `-Force`.
5. Observe what changed in the generated files.
6. Then do the opposite: add a property in the code-first model and create a migration.
7. Compare the two workflows carefully.
