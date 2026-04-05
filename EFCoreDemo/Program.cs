using EFCoreDemo.Configuration;
using EFCoreDemo.Examples;

namespace EFCoreDemo;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var command = (args.FirstOrDefault() ?? "help").Trim().ToLowerInvariant();
        var connectionString = ConnectionStringResolver.Resolve();
        var runner = new StudyRunner(connectionString);

        try
        {
            return command switch
            {
                "help" => await runner.PrintHelpAsync(),
                "info" => await runner.PrintConnectionInfoAsync(),
                "canconnect" => await runner.CanConnectAsync(),
                "migrate" => await runner.ApplyMigrationsAsync(),
                "seed" => await runner.SeedSampleDataAsync(),
                "basic" => await runner.RunBasicCrudAsync(),
                "relationships" => await runner.RunRelationshipExamplesAsync(),
                "json" => await runner.RunJsonExamplesAsync(),
                "rawsql" => await runner.RunRawSqlExamplesAsync(),
                "transaction" => await runner.RunTransactionExampleAsync(),
                "all" => await runner.RunFullStudyGuideAsync(),
                _ => await runner.PrintUnknownCommandAsync(command)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("The study runner stopped because of an error.");
            Console.WriteLine(ex.Message);

            if (ex.InnerException is not null)
            {
                Console.WriteLine(ex.InnerException.Message);
            }

            Console.WriteLine("Check your connection string and database availability, then try the command again.");
            return 1;
        }
    }
}
