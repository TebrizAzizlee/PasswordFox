using AuthServer.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;


namespace AuthServer.Infrastructure.Persistence.Seeds;
public static class SeedExtensions
{
    public static async Task SeedDatabaseAsync(
        this IServiceProvider services)
    {
        using var scope =
            services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AuthServerDbContext>();

        await PermissionSeed
            .SeedAsync(context);

        await RoleSeed
            .SeedAsync(context);

        await RolePermissionSeed
            .SeedAsync(context);
    }
}