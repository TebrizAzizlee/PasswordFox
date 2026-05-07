using AuthServer.Application.Services;
using FluentValidation;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Auth
{
 public sealed record RevokeRefreshTokenCommand(string RefreshToken)
    : IRequest<ServiceResult>;
    public sealed class RevokeRefreshTokenCommandValidator
    : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token daxil edilməlidir");
        }
    }
    public sealed class RevokeRefreshTokenCommandHandler(
     IAuthenticationService authenticationService,IUserContext userContext)
     : IRequestHandler<RevokeRefreshTokenCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            RevokeRefreshTokenCommand request,
            CancellationToken cancellationToken)
        {

            await authenticationService
                .RevokeRefreshTokenAsync(request.RefreshToken,userContext.GetUserId(), cancellationToken);

            // həmişə success qaytar (security üçün)
            return ServiceResult.Success();
        }
    }
}
