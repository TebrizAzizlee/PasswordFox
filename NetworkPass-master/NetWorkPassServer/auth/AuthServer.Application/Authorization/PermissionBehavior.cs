using AuthServer.Application.Services;
using AuthServer.Domain.Permissions;
using SharedLibrary.Middlewares;
using TS.MediatR;

namespace AuthServer.Application.Authorization;

public sealed class PermissionBehavior<TRequest, TResponse>(
    IUserContext userContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request
            .GetType()
            .GetCustomAttributes(typeof(PermissionAttribute), true)
            .FirstOrDefault() is not PermissionAttribute permissionAttribute)
        {
            return await next();
        }

        var userId = userContext.GetUserId();

        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Authentication required");
        }

        var userPermissions = userContext
            .GetPermissions();

        // 🔥 SUPER ADMIN BYPASS

        if (userPermissions.Contains(
            PermissionsView.SuperAdmin))
        {
            return await next();
        }

        // 🔥 PERMISSION CHECK

        if (!userPermissions.Contains(
            permissionAttribute.Permission))
        {
            throw new AuthorizationException(
                $"Required permission: {permissionAttribute.Permission}");
        }

        return await next();
    }
}