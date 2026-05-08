using AuthServer.Domain.Permissions;
using AuthServer.Domain.Roles;
using SharedLibrary.Abstractions.Entity;

namespace AuthServer.Domain.RolePermissions;

public sealed class RolePermission : Entity
{
    public IdentityId RoleId { get; private set; } = default!;

    public Role Role { get; private set; } =default!;

    public IdentityId PermissionId { get; private set; }= default!;

    public Permission Permission { get; private set; }= default!;
       

    private RolePermission() { }

    public RolePermission(
        IdentityId roleId,
        IdentityId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
