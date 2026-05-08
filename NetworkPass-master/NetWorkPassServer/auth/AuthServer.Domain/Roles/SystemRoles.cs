using AuthServer.Domain.Roles.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.Roles;


public static class SystemRoles
{
    public static readonly RoleName User =
        new("User");

    public static readonly RoleName Admin =
        new("Admin");

    public static readonly RoleName Support =
        new("Support");
}