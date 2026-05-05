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
            .AsNoTracking() // 🔥 read-only
            .FirstOrDefaultAsync(x => x.TokenHash == refreshToken, ct);
    }

    public async Task<List<LoginToken>> GetActiveByUserIdAsync(
     Guid userId,
     CancellationToken cancellationToken = default)
    {
        return await _context.Set<LoginToken>()
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> TryDeactivateAsync(string tokenHash, CancellationToken ct = default)
    {
        var affected = await _context.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE LoginTokens
        SET IsActive = 0, RevokedAt = {DateTimeOffset.UtcNow}
        WHERE TokenHash = {tokenHash} AND IsActive = 1
    ", ct);

        return affected > 0;
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
}
