using System.Text.Json;
using Npgsql;

namespace EFCoreDemo.Configuration;

public static class ConnectionStringResolver
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=efcore_study;Username=postgres;Password=devpassword";

    public static string Resolve()
    {
        var fromEnvironment = ResolveFromEnvironment();
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        var fromFile = ResolveFromLocalSettingsFile();
        if (!string.IsNullOrWhiteSpace(fromFile))
        {
            return fromFile;
        }

        return DefaultConnectionString;
    }

    public static string Describe(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        return $"Host={builder.Host};Port={builder.Port};Database={builder.Database};Username={builder.Username};SSL Mode={builder.SslMode}";
    }

    private static string? ResolveFromEnvironment()
    {
        var candidates = new[]
        {
            Environment.GetEnvironmentVariable("EFCOREDEMO_CONNECTION_STRING"),
            Environment.GetEnvironmentVariable("PG_CONNECTION_STRING")
        };

        return candidates.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();
    }

    private static string? ResolveFromLocalSettingsFile()
    {
        foreach (var path in GetCandidateFiles())
        {
            if (!File.Exists(path))
            {
                continue;
            }

            var json = File.ReadAllText(path);
            using var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("ConnectionString", out var directValue) &&
                directValue.ValueKind == JsonValueKind.String)
            {
                return directValue.GetString()?.Trim();
            }

            if (document.RootElement.TryGetProperty("ConnectionStrings", out var section) &&
                section.ValueKind == JsonValueKind.Object &&
                section.TryGetProperty("Default", out var nestedValue) &&
                nestedValue.ValueKind == JsonValueKind.String)
            {
                return nestedValue.GetString()?.Trim();
            }
        }

        return null;
    }

    private static IEnumerable<string> GetCandidateFiles()
    {
        var directories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddDirectoryTree(Directory.GetCurrentDirectory(), directories);
        AddDirectoryTree(AppContext.BaseDirectory, directories);

        foreach (var directory in directories)
        {
            yield return Path.Combine(directory, "appsettings.local.json");
            yield return Path.Combine(directory, "appsettings.example.json");
            yield return Path.Combine(directory, "EFCoreDemo", "appsettings.local.json");
            yield return Path.Combine(directory, "EFCoreDemo", "appsettings.example.json");
        }
    }

    private static void AddDirectoryTree(string? startDirectory, ISet<string> directories)
    {
        var directory = startDirectory;

        while (!string.IsNullOrWhiteSpace(directory))
        {
            directories.Add(directory);
            directory = Directory.GetParent(directory)?.FullName;
        }
    }
}
