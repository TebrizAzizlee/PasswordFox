

using SharedLibrary.Abstractions.Entity;
using System.Linq.Expressions;

namespace AuthServer.Domain.UserRoles;
public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(IdentityId id, CancellationToken cancellationToken = default);

    Task<UserRole?> GetAsync(IdentityId userId, IdentityId roleId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(IdentityId userId, IdentityId roleId, CancellationToken cancellationToken = default);

    Task<List<UserRole>> GetByUserIdAsync(IdentityId userId, CancellationToken cancellationToken = default);

    Task<List<UserRole>> GetByRoleIdAsync(IdentityId roleId, CancellationToken cancellationToken = default);

    Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);
    

    void Remove(UserRole userRole);

    IQueryable<UserRole> Where(Expression<Func<UserRole, bool>> predicate);

}
