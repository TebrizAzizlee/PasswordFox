using AuthServer.Application.Services;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Constants;
using SharedLibrary.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using static AuthServer.Infrastructure.Context.AuthServerDbContext;
using static System.Net.WebRequestMethods;

namespace AuthServer.Infrastructure.Services;
public class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
{
    private readonly IHttpContextAccessor _http=http;

    public Guid UserId
    {
        get
        {
            var user = _http.HttpContext?.User;

            var claim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(claim, out var id)
                ? id
                : SystemUser.Id;
        }
    }

    public string? FullName =>
        _http.HttpContext?.User?.FindFirst("name")?.Value;
}

