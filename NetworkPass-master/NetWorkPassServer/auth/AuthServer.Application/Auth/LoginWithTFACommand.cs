using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using AuthServer.Domain.Users;
using FluentValidation;
using GenericRepository;
using SharedLibrary;
using SharedLibrary.Security;
using System.Net;
using TS.MediatR;

namespace AuthServer.Application.Auth;
public sealed record LoginWithTFACommand(string PendingToken, string TFACode) : IRequest<ServiceResult<LoginResult>>;

public sealed class LoginWithTFACommandValidator
    : AbstractValidator<LoginWithTFACommand>
{
    public LoginWithTFACommandValidator()
    {
        RuleFor(x => x.PendingToken)
            .NotEmpty();

        RuleFor(x => x.TFACode)
            .NotEmpty()
            .Length(6);
    }
}

public sealed class LoginWithTFACommandHandler(IUserRepository userRepository, ITokenService tokenService, IUnitOfWork unitOfWork) : IRequestHandler<LoginWithTFACommand, ServiceResult<LoginResult>>

{
    public async Task<ServiceResult<LoginResult>> Handle(LoginWithTFACommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        // 🔥 HASH PENDING TOKEN

        var pendingTokenHash =
            TokenHashHelper.Hash(
                request.PendingToken);

        //GET USER
        var user = await userRepository.GetPendingTfaUserAsync(request.PendingToken,now, cancellationToken);


        

        if (user is null)
            return ServiceResult<LoginResult>.Failure(
               "InvalidChallenge",
                "Authentication challenge is invalid or expired",
                HttpStatusCode.Unauthorized
            );
        if (user.IsDeleted)
        {
            return ServiceResult<LoginResult>.Failure(
                "UserDeleted",
                "User deleted",
                HttpStatusCode.BadRequest);
        }
       
       
        if (user.TFAIsCompleted)
            return ServiceResult<LoginResult>.Failure(
                "TFA Error",
                "TFA Kodu artıq istifadə edilib.",
                HttpStatusCode.BadRequest
            );
        // 🔥 VERIFY MFA CODE

        var isValidCode = user
            .VerifyTFACode(
                request.TFACode);
        if (!isValidCode)
        {
            return ServiceResult<LoginResult>.Failure(
                "InvalidCode",
                "Two factor authentication code is invalid",
                HttpStatusCode.BadRequest);
        }
        // 🔥 COMPLETE MFA

        user.CompleteTwoFactorAuthentication();
        // 🔥 CLEAR TEMP MFA STATE

        user.ClearPendingTFA();
        // 🔥 RESET FAILED LOGIN STATE

        user.ResetLoginAttempts();

        // 🔥 ISSUE TOKENS

        var token = await tokenService.GenerateTokenAsync(user, cancellationToken);
        await unitOfWork
          .SaveChangesAsync(
              cancellationToken);

        return ServiceResult<LoginResult>
           .Success(
               new LoginResult
               {
                   RequiresTFA = false,
                   Token = token
               });
    }
}