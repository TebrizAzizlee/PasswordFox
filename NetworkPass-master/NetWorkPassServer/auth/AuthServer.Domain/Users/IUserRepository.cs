using AuthServer.Domain.Users.ValueObjects;
using SharedLibrary.Abstractions.Entity;
using System.Linq.Expressions;

namespace AuthServer.Domain.Users;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(IdentityId id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetForAuthenticationAsync(string loginIdentifier, CancellationToken cancellationToken = default);

    Task<User?> GetPendingTfaUserAsync(string pendingTokenHash, DateTimeOffset now, CancellationToken cancellationToken = default);



    Task<User?> GetByResetPasswordTokenAsync(string tokenHash, DateTimeOffset now, CancellationToken cancellationToken = default);



    Task<bool> ExistsByUserNameAsync(UserName userName, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);


    IQueryable<User> Where(Expression<Func<User, bool>> predicate);

    



}
