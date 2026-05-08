namespace AuthServer.Domain.Permissions.ValueObjects;

public sealed record PermissionName
{
    public string Value { get; }

    public PermissionName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Permission name cannot be empty");

        Value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator string(
        PermissionName name)
    {
        return name.Value;
    }
}