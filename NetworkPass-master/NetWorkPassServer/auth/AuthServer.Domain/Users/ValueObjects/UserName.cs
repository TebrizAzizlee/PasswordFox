namespace AuthServer.Domain.Users.ValueObjects;

public sealed record UserName
{
    public string Value { get; }

    public UserName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Username cannot be empty");

        Value = value.Trim();
    }

    public static implicit operator string(
        UserName userName)
    {
        return userName.Value;
    }
}
