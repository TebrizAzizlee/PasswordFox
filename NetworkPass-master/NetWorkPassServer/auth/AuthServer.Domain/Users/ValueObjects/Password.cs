using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace AuthServer.Domain.Users.ValueObjects;

public sealed record Password
{
    private Password()
    {
    }
    public Password(string password)
    {
        //if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        //    throw new ArgumentException("Password must be at least 8 characters long", nameof(password));
        CreatePasswordHash(password);
    }
    [JsonIgnore]
    public byte[] PasswordHash { get; private set; } = default!;
    [JsonIgnore]
    public byte[] PasswordSalt { get; private set; } = default!;

    private void CreatePasswordHash(string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        PasswordSalt = hmac.Key;
        PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
    public bool VerifyPasswordHash(string password)
    {
        using var hmac = new HMACSHA512(PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(PasswordHash);
    }
   
}
