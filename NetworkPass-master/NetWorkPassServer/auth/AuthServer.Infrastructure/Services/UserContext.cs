using AuthServer.Application.Authorization;
using AuthServer.Application.Services;
using Microsoft.AspNetCore.Http;

namespace AuthServer.Infrastructure.Services;
internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    private const string UserNotFoundMessage = "İstifadəçi məlumatları tapılmadı.";
    private const string InvalidUserIdMessage = "İstifadəçi ID-si uyğun formatda deyil.";

    public Guid GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext?? throw new UnauthorizedAccessException(UserNotFoundMessage);
        var user = httpContext.User;
        if (user?.Identity is null || !user.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException(UserNotFoundMessage);
        var userIdValue = user.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;

        if (string.IsNullOrWhiteSpace(userIdValue))
            throw new UnauthorizedAccessException(UserNotFoundMessage);
        if (!Guid.TryParse(userIdValue, out var userId))
            throw new UnauthorizedAccessException(
    InvalidUserIdMessage);
        return userId;
    }


    public IReadOnlyCollection<string> GetPermissions()
    {
        return httpContextAccessor
            .HttpContext?
            .User
            .FindAll(CustomClaimTypes.Permission)
            .Select(x => x.Value)
            .Distinct()
            .ToList()
            ?? [];
    }
}
