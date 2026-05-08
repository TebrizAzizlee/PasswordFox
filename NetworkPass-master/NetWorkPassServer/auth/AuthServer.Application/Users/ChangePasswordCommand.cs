using AuthServer.Domain.LoginTokens;
using AuthServer.Domain.Users;
using AuthServer.Domain.Users.ValueObjects;
using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Users;
public sealed record ChangePasswordCommand(
    Guid UserId,
    Guid CurrentUserId,
    string CurrentPassword,
    string NewPassword)
    : IRequest<ServiceResult>;

public sealed class ChangePasswordCommandValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.CurrentUserId)
            .NotEmpty();

        RuleFor(x => x.CurrentPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .MinimumLength(8)
            .MaximumLength(128);

        RuleFor(x => x.NewPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .MinimumLength(8)
            .MaximumLength(128);
    }
}



public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    ILoginTokenRepository loginTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePasswordCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 SECURITY
        // only self password change

        if (request.UserId != request.CurrentUserId)
        {
            return ServiceResult.Failure(
                "Forbidden",
                "You cannot change another user's password",
                HttpStatusCode.Forbidden);
        }

        var userId = new IdentityId(request.UserId);

        var user = await userRepository
            .GetByIdAsync(
                userId,
                cancellationToken);

        if (user is null)
        {
            return ServiceResult.Failure(
                "UserNotFound",
                "User not found",
                HttpStatusCode.NotFound);
        }

        if (user.IsDeleted)
        {
            return ServiceResult.Failure(
                "UserDeleted",
                "User deleted",
                HttpStatusCode.BadRequest);
        }

        // 🔥 VERIFY CURRENT PASSWORD

        if (!user.VerifyPassword(request.CurrentPassword))
        {
            return ServiceResult.Failure(
                "InvalidPassword",
                "Current password is incorrect",
                HttpStatusCode.BadRequest);
        }

        Password newPassword;

        try
        {
            newPassword = new Password(
                request.NewPassword);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure(
                "ValidationError",
                ex.Message,
                HttpStatusCode.BadRequest);
        }

        // 🔥 PREVENT SAME PASSWORD

        if (user.VerifyPassword(request.NewPassword))
        {
            return ServiceResult.Failure(
                "SamePassword",
                "New password cannot be same as current password",
                HttpStatusCode.BadRequest);
        }

        // 🔥 CHANGE PASSWORD

        user.ChangePassword(newPassword);

        var now = DateTimeOffset.UtcNow;

        // 🔥 REVOKE ALL ACTIVE REFRESH TOKENS

        await loginTokenRepository
            .Where(x =>
                x.UserId == user.Id &&
                x.RevokedAt == null)
            .ExecuteUpdateAsync(x => x
                .SetProperty(
                    t => t.RevokedAt,
                    now)
                .SetProperty(
                    t => t.RevokedReason,
                    "password-changed"),
                cancellationToken);

        try
        {
            await unitOfWork
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Failure(
                "PasswordChangeFailed",
                "Password change failed",
                HttpStatusCode.Conflict);
        }

        return ServiceResult.Success();
    }
}