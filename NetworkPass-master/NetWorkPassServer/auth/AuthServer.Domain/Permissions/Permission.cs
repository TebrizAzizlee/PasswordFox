using AuthServer.Domain.Permissions.ValueObjects;
using SharedLibrary.Abstractions.Entity;

namespace AuthServer.Domain.Permissions;

public sealed class Permission : Entity
{
    public PermissionName Name { get; private set; } = default!;

    public string? Description { get; private set; }

    private Permission() { }

    public Permission(
        PermissionName name,
        string? description = null)
    {
        Name = name;
        Description = description;
    }



}
