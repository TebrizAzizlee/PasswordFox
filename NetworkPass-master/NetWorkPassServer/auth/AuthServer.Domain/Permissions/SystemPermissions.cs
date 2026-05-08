
namespace AuthServer.Domain.Permissions;

public static class SystemPermissions
{
    public static readonly string[] All =
    [
        PermissionsView.Users.View,
        PermissionsView.Users.Create,
        PermissionsView.Users.Update,
        PermissionsView.Users.Delete,

        PermissionsView.Roles.View,
        PermissionsView.Roles.Create,
        PermissionsView.Roles.Assign,
        PermissionsView.Roles.Remove
    ];
}