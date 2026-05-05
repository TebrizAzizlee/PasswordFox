using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using FluentValidation;
using TS.MediatR;
using SharedLibrary;

namespace AuthServer.Application.Auth;

public sealed record LoginCommand(string LoginIdentifier, string Password) : IRequest<ServiceResult<LoginResult>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginIdentifier)
            .NotEmpty()
            .WithMessage("İstifadəçi adı və ya şifrə düzgün deyil.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("İstifadəçi adı və ya şifrə düzgün deyil.");
    }
}

public sealed class LoginCommandHandler(IAuthenticationService authenticationService) : IRequestHandler<LoginCommand, ServiceResult<LoginResult>>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<ServiceResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.CreateTokenAsync(request.LoginIdentifier, request.Password, cancellationToken);
    }
}