namespace AuthServer.Application.Authorization;

[AttributeUsage(
    AttributeTargets.Class,
    AllowMultiple = true,
    Inherited = true)]
public sealed class PermissionAttribute(
    string permission)
        : Attribute
{
    public string Permission { get; } = permission;
}