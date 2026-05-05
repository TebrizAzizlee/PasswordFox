using AuthServer.Domain.LoginTokens;
using AuthServer.Domain.Users;
using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using SharedLibrary;
using SharedLibrary.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Auth
{
    public sealed record ResetPasswordCommand(
     string Token,
     string NewPassword,
     bool LogoutAllDevices
 ) : IRequest<ServiceResult>;
    public sealed class ResetPasswordCommandValidator
    : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Reset token is required.")
                .MinimumLength(32)
                .WithMessage("Invalid reset token.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .MaximumLength(64)
                .WithMessage("Password must not exceed 64 characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.LogoutAllDevices)
                .NotNull();
        }
    }
    internal sealed class ResetPasswordCommandHandler(
        IUserRepository userRepository,
        ILoginTokenRepository loginTokenRepository,
        IUnitOfWork unitOfWork
    ) : IRequestHandler<ResetPasswordCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
    ResetPasswordCommand request,
    CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;
            var tokenHash = TokenHashHelper.Hash(request.Token);

            var user = await userRepository.FirstOrDefaultAsync(
                x => x.ResetPasswordTokenHash == tokenHash &&
                     x.ResetPasswordTokenExpiresAt > now,
                ct);

            if (user is null)
            {
                return ServiceResult.Failure(
                    "InvalidToken",
                    "Reset password token is invalid or expired.",
                    HttpStatusCode.BadRequest);
            }

            // 🔥 eyni password check
            if (user.VerifyPassword(request.NewPassword))
            {
                return ServiceResult.Failure(
                    "SamePassword",
                    "New password must be different.",
                    HttpStatusCode.BadRequest);
            }

            user.ResetPassword(request.NewPassword);
            user.ClearResetPasswordToken();

            if (request.LogoutAllDevices)
            {
                await loginTokenRepository.DeactivateAllByUserIdAsync(user.Id,ct);
            }

            await unitOfWork.SaveChangesAsync(ct);

            return ServiceResult.Success();
        }

    }
}

