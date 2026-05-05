using AuthServer.Application.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        var userIdValue = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdValue))
            throw new UnauthorizedAccessException(UserNotFoundMessage);
        if (!Guid.TryParse(userIdValue, out var userId))
            throw new FormatException(InvalidUserIdMessage);
        return userId;
    }

    public string[] GetUserPermissions()
    {
        throw new NotImplementedException();
    }

    public bool IsAdmin()
    {
        throw new NotImplementedException();
    }
}
