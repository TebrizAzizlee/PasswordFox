using SharedLibrary.Abstractions.Entity;
using System.ComponentModel.DataAnnotations;

public sealed class LoginToken
{
    [Timestamp]
    public byte[] Version { get; private set; } = default!;
    public IdentityId Id { get; private set; } = default!;
    public IdentityId UserId { get; private set; } = default!;
    public Guid TokenFamilyId { get; private set; }=default!;
    public IdentityId? ParentTokenId { get; private set; }
     public string TokenHash { get; private set; }= default!;
    public DateTimeOffset CreatedAt { get; private set; } = default!;
    public DateTimeOffset ExpiresAt { get; private set; } = default!;
 public DateTimeOffset? RevokedAt { get; private set; } = default!;
    public string? RevokedReason { get; private set; } = default!;

    public LoginToken? ParentToken { get; private set; }

    private LoginToken() { }

    public LoginToken(
        string tokenHash,
        IdentityId userId,
        Guid tokenFamilyId,
        DateTimeOffset expiresAt,
        IdentityId? parentTokenId=null)
    {
        Id = new IdentityId(Guid.CreateVersion7());
        TokenFamilyId = tokenFamilyId;
        ParentTokenId = parentTokenId;
        TokenHash = tokenHash.Trim();
        UserId = userId;
       ExpiresAt=expiresAt;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return now >= ExpiresAt;
    }

    public bool IsRevoked()
        => RevokedAt != null;

    public bool IsValid(DateTimeOffset now)
    {
        return !IsExpired(now) && !IsRevoked();
    }

    public void Revoke(string reason,DateTimeOffset now)
    {
        if (RevokedAt != null)
            return;

        RevokedAt = now;
        RevokedReason = reason;
        
    }

    
}