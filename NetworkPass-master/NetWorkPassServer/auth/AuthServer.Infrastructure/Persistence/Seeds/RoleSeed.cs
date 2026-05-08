using AuthServer.Domain.Roles;
using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace AuthServer.Infrastructure.Persistence.Seeds;

public static class RoleSeed
{
    public static async Task SeedAsync(
        AuthServerDbContext context,
        CancellationToken cancellationToken = default)
    {
        var adminRoleExists = await context
            .Set<Role>()
            .AnyAsync(
                x => x.Name == SystemRoles.Admin,
                cancellationToken);

        if (!adminRoleExists)
        {
            var adminRole = new Role(
                SystemRoles.Admin,
                "System administrator");

            await context
                .Set<Role>()
                .AddAsync(
                    adminRole,
                    cancellationToken);
        }

        var userRoleExists = await context
            .Set<Role>()
            .AnyAsync(
                x => x.Name == SystemRoles.User,
                cancellationToken);

        if (!userRoleExists)
        {
            var userRole = new Role(
                SystemRoles.User,
                "Default user role");

            await context
                .Set<Role>()
                .AddAsync(
                    userRole,
                    cancellationToken);
        }

        await context
            .SaveChangesAsync(cancellationToken);
    }
}