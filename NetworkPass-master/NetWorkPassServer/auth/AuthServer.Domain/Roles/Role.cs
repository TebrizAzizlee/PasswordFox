using AuthServer.Domain.RolePermissions;
using AuthServer.Domain.Roles.ValueObjects;
using AuthServer.Domain.UserRoles;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.Roles;
public sealed class Role:Entity
{
    public RoleName Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();


    private Role() { }

    public Role(
       RoleName name,
       string? description,
       bool isSystem = false)
    {
        Name = name;
        Description = description;
        IsSystem = isSystem;
    }
    public void Update(
       RoleName name,
       string? description)
    {
        if (IsSystem)
            throw new Exception("System role cannot be modified");

        Name = name;
        Description = description;
    }
}
