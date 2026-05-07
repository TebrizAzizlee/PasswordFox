using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using FluentValidation;
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


    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>

    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required");
        }
    }

    public sealed class RefreshTokenCommandHandler(IAuthenticationService _authenticationService) : IRequestHandler<RefreshTokenCommand, ServiceResult<TokenDto>>
    {
        

        public async Task<ServiceResult<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenResult = await _authenticationService.CreateTokenByRefreshTokenAsync(request.RefreshToken, cancellationToken);
            return tokenResult;
        }
    }

}
