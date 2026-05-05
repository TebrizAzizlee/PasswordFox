using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Auth
{
    public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<ServiceResult<TokenDto>>;




    public sealed class RefreshTokenCommandHandler(IAuthenticationService authenticationService) : IRequestHandler<RefreshTokenCommand, ServiceResult<TokenDto>>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<ServiceResult<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenResult = await _authenticationService.CreateTokenByRefreshTokenAsync(request.RefreshToken, cancellationToken);
            return tokenResult;
        }
    }

}
