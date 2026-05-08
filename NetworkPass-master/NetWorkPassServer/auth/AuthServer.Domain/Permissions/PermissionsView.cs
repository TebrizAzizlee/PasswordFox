using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.Permissions;
public static class PermissionsView
{
    public const string SuperAdmin =
        "super-admin";

    public static class Users
    {
        public const string View =
            "users.view";

        public const string Create =
            "users.create";

        public const string Update =
            "users.update";

        public const string Delete =
            "users.delete";
    }

    public static class Roles
    {
        public const string View =
            "roles.view";

        public const string Create =
            "roles.create";

        public const string Assign =
            "roles.assign";

        public const string Remove =
            "roles.remove";
    }
}
