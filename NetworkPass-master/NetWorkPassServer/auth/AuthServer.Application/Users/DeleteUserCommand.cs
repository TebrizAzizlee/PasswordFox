using AuthServer.Domain.LoginTokens;
using AuthServer.Domain.Users;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Constants;
using System.Net;
using TS.MediatR;

namespace AuthServer.Application.Users;

public sealed record DeleteUserCommand(
    Guid UserId, Guid CurrentUserId)
    : IRequest<ServiceResult>;

public sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    ILoginTokenRepository loginTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 SELF DELETE PROTECTION

        if (request.UserId ==
            request.CurrentUserId)
        {
            return ServiceResult.Failure(
                "SelfDeleteForbidden",
                "You cannot delete your own account",
                HttpStatusCode.BadRequest);
        }
        var userId = new IdentityId(request.UserId);
        var currentUserId =
           new IdentityId(request.CurrentUserId);

        //GET USER
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
        // 🔥 ALREADY DELETED
        if (user.IsDeleted)
        {
            return ServiceResult.Success();
        }
        // 🔥 PROTECT ROOT ADMIN

        if (user.UserName.Value == "admin")
        {
            return ServiceResult.Failure(
                "ProtectedAccount",
                "Default admin account cannot be deleted",
                HttpStatusCode.BadRequest);
        }
        var now = DateTimeOffset.UtcNow;

        // 🔥 SOFT DELETE
        user.Delete(
            new IdentityId(request.CurrentUserId),
            now);

        // 🔥 REVOKE ALL ACTIVE TOKENS
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
                    "user-deleted"),
                cancellationToken);

        try
        {
            await unitOfWork
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Failure(
                "DeleteFailed",
                "User delete failed",
                HttpStatusCode.Conflict);
        }

        return ServiceResult.Success();
    }
}