namespace AuthServer.Domain.Roles.ValueObjects;

public sealed record RoleName
{
    public string Value { get; }

    public RoleName(string value)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Role name required");

        if (value.Length > 64)
            throw new ArgumentException("Role name too long");

        Value = value;
    }

    public static implicit operator string(RoleName name)
        => name.Value;
}
