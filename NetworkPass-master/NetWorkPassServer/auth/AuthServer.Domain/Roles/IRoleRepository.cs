

using AuthServer.Domain.Roles.ValueObjects;
using SharedLibrary.Abstractions.Entity;
using System.Linq.Expressions;
using System.Threading;

namespace AuthServer.Domain.Roles;
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(IdentityId id, CancellationToken cancellationToken = default);

    Task<Role?> GetByNameAsync(RoleName nameCancellationToken,CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(RoleName name, CancellationToken cancellationToken = default);

    Task AddAsync(Role role, CancellationToken cancellationToken = default);

    IQueryable<Role> Where(Expression<Func<Role, bool>> predicate);

}
