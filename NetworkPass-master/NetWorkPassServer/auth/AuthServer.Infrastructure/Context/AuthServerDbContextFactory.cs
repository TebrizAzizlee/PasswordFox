using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Context;
public class AuthServerDbContextFactory
    : IDesignTimeDbContextFactory<AuthServerDbContext>
{
    public AuthServerDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(),"..", "AuthServer.Infrastructure");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("SqlServer");
       
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Connection string is NULL");

        var optionsBuilder = new DbContextOptionsBuilder<AuthServerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AuthServerDbContext(optionsBuilder.Options);
    }
}
