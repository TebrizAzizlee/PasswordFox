using AuthServer.Application;
using AuthServer.Infrastructure;
using AuthServer.WEBapi;
using AuthServer.WEBapi.Modules;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using Serilog;
using SharedLibrary.Configurations;
using SharedLibrary.Middlewares;
using AuthServer.Infrastructure.Persistence.Seeds;
var builder = WebApplication.CreateBuilder(args);

#region 🔹 Serilog (Logging sistemi)
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});
#endregion
#region 🔹 Global Exception Handler (Qlobal xətaların idarəsi)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails();
#endregion

#region 🔹 JWT Authentication & Authorization
var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

builder.Services.AddCustomJwtAuth(tokenOptions!);

builder.Services.AddAuthorization();
#endregion

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

#region 🔹 Rate Limiter (Sorgu limitləri)
builder.Services.AddRateLimiter(cfg =>
{
    // Default reject cavabı
    cfg.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = 429,
            message = "Çox tez sorğu göndərdiniz. Zəhmət olmasa bir müddət gözləyin."
        }, token);
    };

    // Ümumi sorğu limiti
    cfg.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.QueueLimit = 100;
        opt.Window = TimeSpan.FromSeconds(5);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });

    // Login üçün xüsusi limit
    cfg.AddFixedWindowLimiter("login-fixed", opt =>
    {
        opt.PermitLimit = 5;
        opt.QueueLimit = 2;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });

    // Forgot password
    cfg.AddFixedWindowLimiter("forgot-password-fixed", opt =>
    {
        opt.PermitLimit = 2;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });

    // Reset password
    cfg.AddFixedWindowLimiter("reset-password-fixed", opt =>
    {
        opt.PermitLimit = 2;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });

    // Reset token yoxlama
    cfg.AddFixedWindowLimiter("CheckResetPasswordToken", opt =>
    {
        opt.PermitLimit = 2;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
    cfg.AddFixedWindowLimiter("refresh-token-fixed", opt =>
    {
        opt.PermitLimit = 5; // istədiyin limit
        opt.QueueLimit = 2;  // istədiyin queue limit
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
});
#endregion

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));

builder.Services.AddHostedService<CheckLoginTokenBackgroundService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddResponseCompression(opt => opt.EnableForHttps = true);

var app = builder.Build();
await app.Services.SeedDatabaseAsync();
await app.CreateFirstUser();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseRateLimiter();

app.UseResponseCompression();

app.UseAuthentication();
app.UseAuthorization();
//app.UseRouting();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference();

}


app.MapControllers().RequireRateLimiting("fixed");

app.MapAuthEndpoint().RequireCors("Frontend"); 

// SPA fallback
//app.MapFallbackToFile("index.html");
// İlk istifadəçi yaratawait



app.Run();