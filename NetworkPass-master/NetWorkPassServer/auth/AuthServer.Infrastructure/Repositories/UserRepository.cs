using AuthServer.Domain;
using AuthServer.Domain.Users;
using AuthServer.Infrastructure.Abstractions;
using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Repositories;
internal sealed class UserRepository(AuthServerDbContext context) : AuditableRepository<User, AuthServerDbContext>(context), IUserRepository
{
    private readonly AuthServerDbContext _dbContext = context;
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        return await _dbContext.Users
            .AsNoTracking() // yalnız oxuma üçün performans artırır
            .FirstOrDefaultAsync(u => u.UserName.Value == username, cancellationToken);
    }
}
