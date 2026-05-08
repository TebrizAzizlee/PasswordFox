using AuthServer.Domain.Permissions;
using AuthServer.Domain.Permissions.ValueObjects;
using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace AuthServer.Infrastructure.Persistence.Seeds;
public static class PermissionSeed
{
    public static async Task SeedAsync(
        AuthServerDbContext context,
        CancellationToken cancellationToken = default)
    {
        var existingPermissions = await context
            .Set<Permission>()
            .Select(x => x.Name.Value)
            .ToListAsync(cancellationToken);

        var newPermissions = SystemPermissions.All
            .Where(x => !existingPermissions.Contains(x))
            .Select(x => new Permission(new PermissionName(x)))
            .ToList();

        if (newPermissions.Count == 0)
        {
            return;
        }

        await context
            .Set<Permission>()
            .AddRangeAsync(
                newPermissions,
                cancellationToken);

        await context
            .SaveChangesAsync(cancellationToken);
    }
}
