using GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.LoginTokens;
public interface ILoginTokenRepository: IRepository<LoginToken>
{
    Task<LoginToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<List<LoginToken>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<LoginToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
   
    Task<int> DeactivateExpiredTokensAsync(CancellationToken ct = default);
    Task<int> DeactivateAllByUserIdAsync(Guid userId,CancellationToken cancellationToken);
    Task<bool> TryDeactivateAsync(string tokenHash, CancellationToken ct = default);
    
}
