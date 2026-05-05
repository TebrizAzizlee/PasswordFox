using AuthServer.Application.Dtos;
using AuthServer.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Services;
public interface ITokenService
{
    Task<TokenDto> GenerateTokenAsync(User user, CancellationToken cancellationToken);
    string CreateRefreshToken();
}
