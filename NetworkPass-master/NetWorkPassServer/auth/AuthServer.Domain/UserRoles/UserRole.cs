using AuthServer.Domain.Roles;
using AuthServer.Domain.Users;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.UserRoles;
public sealed class UserRole:Entity
{
    public IdentityId UserId { get; private set; } = default!;
    public IdentityId RoleId { get; private set; } = default!;

    public User User { get; private set; } = default!;
    public Role Role { get; private set; } = default!;
    
    private UserRole() { }
    public UserRole(
       IdentityId userId,
       IdentityId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
