using EFCoreDemo.Data;
using Microsoft.EntityFrameworkCore.Design;

namespace EFCoreDemo.Configuration;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        return new AppDbContext(ConnectionStringResolver.Resolve());
    }
}
