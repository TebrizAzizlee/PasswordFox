using AuthServer.Domain.UserRoles;
using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Repositories;
internal sealed class UserRoleRepository(AuthServerDbContext _context)
    : IUserRoleRepository
{
   

    public async Task<UserRole?> GetByIdAsync(IdentityId id, CancellationToken cancellationToken = default)

    {
        return await _context.Set<UserRole>()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<UserRole?> GetAsync(IdentityId userId, IdentityId roleId, CancellationToken cancellationToken = default)

    {
        return await _context.Set<UserRole>()
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.RoleId == roleId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(IdentityId userId, IdentityId roleId, CancellationToken cancellationToken = default)

    {
        return await _context.Set<UserRole>()
            .AnyAsync(
                x => x.UserId == userId &&
                     x.RoleId == roleId,
                cancellationToken);
    }

    public async Task<List<UserRole>> GetByUserIdAsync(
        IdentityId userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Where(x => x.UserId == userId)
            .Include(x => x.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserRole>> GetByRoleIdAsync(
        IdentityId roleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Where(x => x.RoleId == roleId)
            .Include(x => x.User)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<UserRole>()
            .AddAsync(userRole, cancellationToken);
    }

    public void Remove(UserRole userRole)
    {
        _context.Set<UserRole>()
            .Remove(userRole);
    }

    public IQueryable<UserRole> Where(
        Expression<Func<UserRole, bool>> predicate)
    {
        return _context.Set<UserRole>()
            .Where(predicate);
    }
}
