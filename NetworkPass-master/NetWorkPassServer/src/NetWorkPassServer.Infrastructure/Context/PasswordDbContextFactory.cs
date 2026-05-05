using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace NetWorkPassServer.Infrastructure.Context;
public class PasswordDbContextFactory
    : IDesignTimeDbContextFactory<PasswordDbContext>
{
    public PasswordDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "NetWorkPassServer.Infrastructure");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("SqlServer");

        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Connection string is NULL");
       
        var optionsBuilder = new DbContextOptionsBuilder<PasswordDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new PasswordDbContext(optionsBuilder.Options);
    }
}
