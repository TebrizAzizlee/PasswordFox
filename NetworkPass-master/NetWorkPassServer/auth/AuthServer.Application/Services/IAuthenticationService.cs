using AuthServer.Application.Dtos;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Services;
public interface IAuthenticationService
{
    Task<ServiceResult<LoginResult>> CreateTokenAsync(string UserName, string password, CancellationToken cancellationToken);
    Task<ServiceResult<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<ServiceResult<string>> RevokeRefreshTokenAsync(string refreshToken, Guid currentUserId, CancellationToken cancellationToken = default);
}
