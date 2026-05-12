using AuthServer.Application.Authorization;
using AuthServer.Application.Services;
using AuthServer.Application.Users;
using AuthServer.Domain.Permissions;
using AuthServer.WEBapi.Extentions;
using TS.MediatR;
using Volo.Abp.Users;

namespace AuthServer.WEBapi.Modules;

public static class UsersModule
{
    public static void MapUsersEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/users")
            .RequireAuthorization();

        // ================= GET USERS =================

        group.MapGet("/",
            async (
                int page,
                int pageSize,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUsersQuery(
                    page,
                    pageSize);

                var result = await sender
                    .Send(
                        query,
                        cancellationToken);

                return Results.Ok(result);
            })
            .RequireAuthorization(
                policy =>
                    policy.RequireClaim(
                        CustomClaimTypes.Permission,
                        PermissionsView.Users.View));

        // ================= GET USER BY ID =================

        group.MapGet("/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query =
                    new GetUserByIdQuery(id);

                var result = await sender
                    .Send(
                        query,
                        cancellationToken);

                return Results.Ok(result);
            })
            .RequireAuthorization(
                policy =>
                    policy.RequireClaim(
                        CustomClaimTypes.Permission,
                        PermissionsView.Users.View));

        // ================= CREATE USER =================

        group.MapPost("/",
            async (
                CreateUserCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender
                    .Send(
                        command,
                        cancellationToken);

                return Results.Ok(result);
            })
            .RequireAuthorization(
                policy =>
                    policy.RequireClaim(
                        CustomClaimTypes.Permission,
                        PermissionsView.Users.Create));

        // ================= UPDATE USER =================

        group.MapPut("/{id:guid}",
            async (
                Guid id,
                UpdateUserCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var request =
                    command with { UserId = id };

                var result = await sender
                    .Send(
                        request,
                        cancellationToken);

                return Results.Ok(result);
            })
            .RequireAuthorization(
                policy =>
                    policy.RequireClaim(
                        CustomClaimTypes.Permission,
                        PermissionsView.Users.Update));

        // ================= DELETE USER =================

        group.MapDelete("/{id:guid}",
            async (
                Guid id,
                IUserContext userContext,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var currentUserId =
           userContext.GetUserId();
                var command =
                    new DeleteUserCommand(id,currentUserId);

                var result = await sender
                    .Send(
                        command,
                        cancellationToken);

                return result.ToResult();
            })
            .RequireAuthorization(
                policy =>
                    policy.RequireClaim(
                        CustomClaimTypes.Permission,
                        PermissionsView.Users.Delete));

        // ================= CHANGE PASSWORD =================

        group.MapPost("/{id:guid}/change-password",
            async (
                Guid id,
                ChangePasswordCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var request =
                    command with { UserId = id };

                var result = await sender
                    .Send(
                        request,
                        cancellationToken);

                return Results.Ok(result);
            });
    }
}