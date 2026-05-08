using AuthServer.Domain.Permissions;
using AuthServer.Domain.RolePermissions;
using AuthServer.Domain.Roles;
using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace AuthServer.Infrastructure.Persistence.Seeds;


public static class RolePermissionSeed
{
    public static async Task SeedAsync(
        AuthServerDbContext context,
        CancellationToken cancellationToken = default)
    {
        var adminRole = await context
            .Set<Role>()
            .FirstOrDefaultAsync(
                x => x.Name == SystemRoles.Admin,
                cancellationToken);

        if (adminRole is null)
        {
            return;
        }

        var permissions = await context
            .Set<Permission>()
            .ToListAsync(cancellationToken);

        var existingPermissionIds = await context
            .Set<RolePermission>()
            .Where(x => x.RoleId == adminRole.Id)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var newRolePermissions = permissions
            .Where(x => !existingPermissionIds.Contains(x.Id))
            .Select(x => new RolePermission(
                adminRole.Id,
                x.Id))
            .ToList();

        if (newRolePermissions.Count == 0)
        {
            return;
        }

        await context
            .Set<RolePermission>()
            .AddRangeAsync(
                newRolePermissions,
                cancellationToken);

        await context
            .SaveChangesAsync(cancellationToken);
    }
}