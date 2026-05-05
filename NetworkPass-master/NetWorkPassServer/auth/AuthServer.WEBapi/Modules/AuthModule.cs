using AuthServer.Application.Auth;
using AuthServer.Application.Dtos;
using AuthServer.Domain.LoginTokens;
using AuthServer.Domain.Users;
using AuthServer.WEBapi.Extentions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;
using System.Net;
using System.Security.Claims;
using TS.MediatR;

namespace AuthServer.WEBapi.Modules;

public static class AuthModule
{
   
    public static void MapAuthEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("/auth").WithTags("Auth");

       
        // 🔐 Cookie helper
        static void SetAuthCookies(HttpContext ctx, TokenDto token)
        {
            //ctx.Response.Cookies.Append("accessToken", token.AccessToken!, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Expires = token.AccessTokenExpiration?.UtcDateTime,
            //    Path = "/"
            //});

            ctx.Response.Cookies.Append("refreshToken", token.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = token.RefreshTokenExpiration?.UtcDateTime,
                Path = "/"
            });
        }
     
        // 🔥 LOGIN
        app.MapPost("/login",
        async (HttpContext ctx, LoginCommand cmd, ISender sender, CancellationToken ct) =>
        {
            var res = await sender.Send(cmd, ct);

            if (!res.IsSuccess || res.Data is null)
            {
                return Results.Problem(
                    title: res.Error?.Title,
                    detail: res.Error?.Detail,
                    statusCode: res.Error?.Status
                );
            }

            if (!res.Data.RequiresTFA && res.Data.Token is not null)
            {
                SetAuthCookies(ctx, res.Data.Token);
                // 🔒 CSRF TOKEN
                var csrfToken = Guid.NewGuid().ToString();
                ctx.Response.Cookies.Append("X-CSRF-TOKEN", csrfToken.ToString(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                });
            }

            return Results.Ok(new
            {
                success=true,
                requiresTfa = res.Data.RequiresTFA,
                accessToken = res.Data.Token?.AccessToken
            });
        })
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        // 🔥 LOGIN WITH TFA
        app.MapPost("/login-with-tfa",
        async (HttpContext ctx, LoginWithTFACommand cmd, ISender sender, CancellationToken ct) =>
        {
            var res = await sender.Send(cmd, ct);

            if (!res.IsSuccess || res.Data is null)
            {
                return Results.Problem(
                    title: res.Error?.Title,
                    detail: res.Error?.Detail,
                    statusCode: res.Error?.Status
                );
            }

            if (res.Data.Token is not null)
            {
                SetAuthCookies(ctx, res.Data.Token);
                // 🔒 YENİ CSRF TOKEN
                var csrfToken = Guid.NewGuid().ToString();
                ctx.Response.Cookies.Append("X-CSRF-TOKEN", csrfToken.ToString(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                });
            }

            return Results.Ok(new { success = true });
        });

        static bool IsValidCsrf(HttpContext ctx)
        {
            if (!ctx.Request.Headers.TryGetValue("X-CSRF-TOKEN", out var csrfHeader))
                return false;

            if (!ctx.Request.Cookies.TryGetValue("X-CSRF-TOKEN", out var csrfCookie))
                return false;

            return csrfHeader == csrfCookie;
        }

        // 🔥 REFRESH TOKEN
        app.MapPost("/refresh-token",
        async (HttpContext ctx, ISender sender, CancellationToken ct) =>
        {
            // 🔒 CSRF CHECK
            if (!IsValidCsrf(ctx))
                return Results.Unauthorized();

            if (!ctx.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
                || string.IsNullOrWhiteSpace(refreshToken))
            {
                return Results.Problem(
                    title: "Unauthorized",
                    detail: "Refresh token missing",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var res = await sender.Send(new RefreshTokenCommand(refreshToken), ct);

            if (!res.IsSuccess || res.Data is null)
            {
                return Results.Problem(
                    title: "Unauthorized",
                    detail: "Invalid refresh token",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            SetAuthCookies(ctx, res.Data);

            return Results.NoContent(); // 🔥 əsas dəyişiklik
        });

        // 🔥 LOGOUT
        app.MapPost("/logout",
        async (HttpContext ctx, ISender sender, CancellationToken ct) =>
        {
            // 🔒 CSRF CHECK
            if (!IsValidCsrf(ctx))
                return Results.Unauthorized();

            if (ctx.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                await sender.Send(new RevokeRefreshTokenCommand(refreshToken), ct);
            }

            ctx.Response.Cookies.Delete("accessToken");
            ctx.Response.Cookies.Delete("refreshToken");
            ctx.Response.Cookies.Delete("X-CSRF-TOKEN");

            return Results.NoContent();
        });

        // 🔥 RESET PASSWORD
        app.MapPost("/reset-password",
        async (HttpContext ctx, ResetPasswordCommand cmd, ISender sender, CancellationToken ct) =>
        {
            // 🔒 CSRF CHECK
            if (!IsValidCsrf(ctx))
                return Results.Unauthorized();

            var res = await sender.Send(cmd, ct);

            if (!res.IsSuccess)
            {
                return Results.Problem(
                    title: res.Error?.Title,
                    detail: res.Error?.Detail,
                    statusCode: res.Error?.Status
                );
            }

            return Results.NoContent();
        });

        // 🔥 ME
        app.MapGet("/me",
        [Authorize]
        (HttpContext ctx) =>
        {
            return Results.Ok(new
            {
                userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier),
                email = ctx.User.FindFirstValue(ClaimTypes.Email),
               userName = ctx.User.FindFirstValue(ClaimTypes.Name)

            });
        });
    }
}