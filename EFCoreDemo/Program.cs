using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EFCoreDemo;

public static class Program
{

    public static async Task<int> myconnection()
    {

        var connectionString = "Host=aws-0-eu-west-1.pooler.supabase.com;" +
        "Database=postgres;" +
        "Username=postgres.dgjhlemxztrwyfvyyxrd;" +
        "Password=ujkabujka31;" +
        "SSL Mode=Require;" +
        "Trust Server Certificate=true";

        CanOpenConnection(connectionString);
        try
        {
            await using var db = new AppDbContext(connectionString!);

            var users = await db.Users.ToListAsync();

            Console.WriteLine("Users:");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Id} - {user.Username} - {user.Email}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return 1;
    }
    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=devdb;Username=postgres;Password=devpassword";

    public static async Task<int> Main()
    {

        await myconnection();

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
