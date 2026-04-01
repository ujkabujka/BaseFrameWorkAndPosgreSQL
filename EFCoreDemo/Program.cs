using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EFCoreDemo;

public static class Program
{
    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=devdb;Username=postgres;Password=devpassword";

    public static int Main()
    {
        Console.WriteLine("PostgreSQL + EF Core study demo");
        Console.WriteLine("===============================\n");

        var connectionString = GetConnectionString();
        Console.WriteLine($"Using database host: {new NpgsqlConnectionStringBuilder(connectionString).Host}");

        if (!CanOpenConnection(connectionString))
        {
            Console.WriteLine("\nCould not connect to PostgreSQL.");
            PrintHelp();
            return 1;
        }

        using var context = new AppDbContext(connectionString);

        Console.WriteLine("\n1) Ensuring database exists...");
        context.Database.EnsureCreated();
        Console.WriteLine("Database is ready.");

        Console.WriteLine("\n2) Seeding sample rows (if needed)...");
        if (!context.Denemes.Any())
        {
            context.Denemes.AddRange(
                new Deneme { Name = "Alice", Height = 165.5, Age = 25 },
                new Deneme { Name = "Bob", Height = 180.2, Age = 30 },
                new Deneme { Name = "Charlie", Height = 175.0, Age = 28 },
                new Deneme { Name = "Diana", Height = 160.3, Age = 22 },
                new Deneme { Name = "Eve", Height = 170.8, Age = 35 });
            context.SaveChanges();
            Console.WriteLine("Sample data inserted.");
        }
        else
        {
            Console.WriteLine("Data already present. Seed skipped.");
        }

        Console.WriteLine("\n3) Query all rows:");
        foreach (var person in context.Denemes.OrderBy(p => p.Name))
        {
            Console.WriteLine($"- {person.Name,-8} | {person.Height,6} cm | {person.Age,2} years");
        }

        Console.WriteLine("\n4) Filter: people taller than 170 cm");
        var tallPeople = context.Denemes.Where(p => p.Height > 170).OrderBy(p => p.Height).ToList();
        foreach (var person in tallPeople)
        {
            Console.WriteLine($"- {person.Name} ({person.Height} cm)");
        }

        Console.WriteLine("\nDone. Tip: set PG_CONNECTION_STRING to switch servers quickly.");
        return 0;
    }

    private static string GetConnectionString()
    {
        return Environment.GetEnvironmentVariable("PG_CONNECTION_STRING")?.Trim() switch
        {
            { Length: > 0 } value => value,
            _ => DefaultConnectionString
        };
    }

    private static bool CanOpenConnection(string connectionString)
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connection check: success.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection check failed: {ex.Message}");
            return false;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("\nQuick options to get a PostgreSQL server:");
        Console.WriteLine("1) Docker (recommended for local study):");
        Console.WriteLine("   docker run --name pg-study -e POSTGRES_PASSWORD=devpassword -e POSTGRES_DB=devdb -p 5432:5432 -d postgres:16");
        Console.WriteLine("2) Free remote PostgreSQL (Neon, Supabase, ElephantSQL alternatives). Copy connection string to PG_CONNECTION_STRING.");
        Console.WriteLine("3) If Docker is unavailable, ask for a managed PostgreSQL instance from your cloud account.");
    }
}
