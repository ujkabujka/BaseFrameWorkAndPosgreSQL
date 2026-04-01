# BaseFrameWorkAndPosgreSQL

Base framework study project for PostgreSQL + C#/.NET.

## Projects in solution

- `EFCoreDemo/EFCoreDemo.csproj`

> If you see `MSB3202 project file was not found`, your `.sln` likely contains stale nested paths.
> This repository now references only the correct path above.

## PostgreSQL Server Details (example local setup)

- **Container Name:** `postgres-dev`
- **Database:** `devdb`
- **Username:** `postgres`
- **Password:** `devpassword`
- **Host:** `localhost`
- **Port:** `5432`
- **Version:** PostgreSQL 16 (recommended)

Start local PostgreSQL with Docker:

```bash
docker run -d --name postgres-dev \
  -e POSTGRES_PASSWORD=devpassword \
  -e POSTGRES_DB=devdb \
  -p 5432:5432 \
  postgres:16
```

Then build and run:

```bash
dotnet build BaseFrameWorkAndPosgreSQL.sln
dotnet run --project EFCoreDemo
```

## Fix for your Windows MSB3202 error

If your Visual Studio/CLI still shows old invalid paths like:

- `EFCoreDemo\\EFCoreDemo\\EFCoreDemo.csproj`
- `EFCoreDemo\\EFCoreDemo\\EFCoreDemo\\EFCoreDemo\\EFCoreDemo.csproj`

run these commands in the repo root:

```bash
dotnet sln BaseFrameWorkAndPosgreSQL.sln remove EFCoreDemo/EFCoreDemo/EFCoreDemo.csproj
dotnet sln BaseFrameWorkAndPosgreSQL.sln remove EFCoreDemo/EFCoreDemo/EFCoreDemo/EFCoreDemo/EFCoreDemo.csproj
dotnet sln BaseFrameWorkAndPosgreSQL.sln add EFCoreDemo/EFCoreDemo.csproj
```
