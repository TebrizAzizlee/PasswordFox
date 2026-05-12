using AuthServer.Domain;
using AuthServer.Domain.Users;
using AuthServer.Domain.Users.ValueObjects;
using AuthServer.Infrastructure.Abstractions;
using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Repositories;
internal sealed class UserRepository(AuthServerDbContext context) : AuditableRepository<User, AuthServerDbContext>(context), IUserRepository
{
    private readonly AuthServerDbContext _dbContext = context;

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext
           .Set<User>()
           .AnyAsync(
               x => x.Email == email,
               cancellationToken);
    }

    public async Task<bool> ExistsByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<User>()
            .AnyAsync(
                x => x.UserName.Value == userName.Value,
                cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext
           .Set<User>()
           .FirstOrDefaultAsync(
               x => x.Email == email,
               cancellationToken);
    }

    public async Task<User?> GetByIdAsync(IdentityId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User?> GetByLoginIdentifierAsync(string loginIdentifier, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByResetPasswordTokenAsync(
    string tokenHash,
    DateTimeOffset now,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<User>()
            .FirstOrDefaultAsync(
                x =>
                    x.ResetPasswordTokenHash == tokenHash &&
                    x.ResetPasswordTokenExpiresAt > now,
                cancellationToken);
    }

    public async Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        return await _dbContext
           .Set<User>()
           .FirstOrDefaultAsync(
               x => x.UserName == userName,
               cancellationToken);
    }

    public async Task<User?> GetForAuthenticationAsync(string loginIdentifier, CancellationToken cancellationToken = default)
    {
        loginIdentifier = loginIdentifier.Trim();

        return await _dbContext
            .Set<User>()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(
                x =>
                    x.Email.Value == loginIdentifier ||
                    x.UserName.Value == loginIdentifier,
                cancellationToken);
    }

    public async Task<User?> GetPendingTfaUserAsync(
     string pendingTokenHash,
     DateTimeOffset now,
     CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<User>()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(
                x =>
                    x.PendingTFATokenHash == pendingTokenHash &&
                    x.TFAExpiresDate != null &&
                    x.TFAExpiresDate > now,
                cancellationToken);
    }
}
