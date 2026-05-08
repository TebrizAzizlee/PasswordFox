using AuthServer.Domain.Roles;
using AuthServer.Domain.Roles.ValueObjects;
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
internal sealed class RoleRepository(AuthServerDbContext _context)
    : IRoleRepository
{
  

    public async Task<Role?> GetByIdAsync(IdentityId id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Role>()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(RoleName name, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Role>()
            .FirstOrDefaultAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(RoleName name, CancellationToken cancellationToken = default)

    {
        return await _context.Set<Role>()
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)

    {
        await _context.Set<Role>()
            .AddAsync(role, cancellationToken);
    }

    public IQueryable<Role> Where(Expression<Func<Role, bool>> predicate)

    {
        return _context.Set<Role>()
            .Where(predicate);
    }
}