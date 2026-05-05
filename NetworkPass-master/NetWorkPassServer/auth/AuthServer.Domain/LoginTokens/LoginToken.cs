using SharedLibrary.Abstractions.Entity;

public sealed class LoginToken:Entity
{
    
    public IdentityId UserId { get; private set; } = default!;
    public string TokenHash { get; private set; } = default!;
    public DateTimeOffset ExpiresAt { get; private set; } = default!;
    public DateTimeOffset? RevokedAt { get; private set; } = default!;
    public string? RevokedReason { get; private set; }
    private LoginToken() { }

    public LoginToken(string tokenHash, IdentityId userId, DateTimeOffset expiresAt)
    {
       
        TokenHash = tokenHash.Trim();
        UserId = userId;
        ExpiresAt = expiresAt;
    }

    public void Deactivate(string reason = "manual")
    {
        if (!IsActive) return;

        SetStatus(false);
        RevokedAt = DateTimeOffset.UtcNow;
        RevokedReason=reason;
    }

    public bool IsExpired() => DateTimeOffset.UtcNow >= ExpiresAt;

    public bool IsValid() => IsActive && !IsExpired();
}