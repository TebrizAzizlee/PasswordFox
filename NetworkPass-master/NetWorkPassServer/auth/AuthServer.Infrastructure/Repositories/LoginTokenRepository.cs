using AuthServer.Domain.LoginTokens;
using AuthServer.Infrastructure.Context;
using GenericRepository;
using Microsoft.EntityFrameworkCore;


namespace AuthServer.Infrastructure.Repositories;
internal sealed class LoginTokenRepository(AuthServerDbContext context) : Repository<LoginToken, AuthServerDbContext>(context), ILoginTokenRepository
{
    private readonly AuthServerDbContext _context = context;

    public async Task<List<LoginToken>> GetAllByUserIdAsync(
    Guid userId,
    CancellationToken ct = default)
    {
        return await _context.Set<LoginToken>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<LoginToken?> GetByRefreshTokenAsync(
    string refreshToken,
    CancellationToken ct = default)
    {
        return await _context.Set<LoginToken>()
            
            .FirstOrDefaultAsync(x => x.TokenHash == refreshToken, ct);
    }

    public async Task<List<LoginToken>> GetActiveByUserIdAsync(
     Guid userId,
     CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await _context.Set<LoginToken>()
               .Where(x =>
                   x.UserId == userId &&
                   x.RevokedAt == null &&
                   x.ExpiresAt > now)
               .ToListAsync(cancellationToken);
    }
    public async Task<bool> TryDeactivateAsync(string tokenHash, CancellationToken ct)
    {
        var affected = await _context.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE LoginTokens
        SET RevokedAt = {DateTime.UtcNow},
            RevokedReason = {"rotated"}
        WHERE TokenHash = {tokenHash}
          AND RevokedAt IS NULL
    ", ct);

        return affected == 1;
    }

    public async Task<int> DeactivateExpiredTokensAsync(CancellationToken ct = default)
    {
        return await _context.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE LoginTokens
        SET IsActive = 0, RevokedAt = {DateTimeOffset.UtcNow}
        WHERE IsActive = 1 AND ExpiresAt < {DateTimeOffset.UtcNow}
    ", ct);
    }



    public async Task<int> DeactivateAllByUserIdAsync(Guid userId,CancellationToken cancellationToken)
    {
        return await _context.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE LoginTokens
        SET IsActive = 0, RevokedAt = {DateTimeOffset.UtcNow}
        WHERE UserId = {userId} AND IsActive = 1
    ");
    }
    public async Task<bool> TryRevokeAsync(
    string tokenHash,
    string reason,
    CancellationToken ct = default)
    {
        var affected = await _context.LoginTokens
            .Where(x =>
                x.TokenHash == tokenHash &&
                x.RevokedAt == null)
            .ExecuteUpdateAsync(x => x
                .SetProperty(t => t.RevokedAt, DateTime.UtcNow)
                .SetProperty(t => t.RevokedReason, reason), ct);

        return affected == 1;
    }
}
