using EFCoreDemo.Configuration;
using EFCoreDemo.Examples;

namespace EFCoreDemo;

public static class Program
{
    public static int Main(string[] args)
    {
        var command = (args.FirstOrDefault() ?? "help").Trim().ToLowerInvariant();
        var connectionString = ConnectionStringResolver.Resolve();
        var runner = new StudyRunner(connectionString);

        try
        {
            return command switch
            {
                "help" => runner.PrintHelp(),
                "info" => runner.PrintConnectionInfo(),
                "canconnect" => runner.CanConnect(),
                "migrate" => runner.ApplyMigrations(),
                "seed" => runner.SeedSampleData(),
                "basic" => runner.RunBasicCrud(),
                "queries" => runner.RunQueryLearningExamples(),
                "relationships" => runner.RunRelationshipExamples(),
                "json" => runner.RunJsonExamples(),
                "rawsql" => runner.RunRawSqlExamples(),
                "transaction" => runner.RunTransactionExample(),
                "all" => runner.RunFullStudyGuide(),
                _ => runner.PrintUnknownCommand(command)
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
