
using Microsoft.AspNetCore.RateLimiting;
using NetWorkPassServer.Application;
using NetWorkPassServer.Infrastructure;
using NetWorkPassServer.WEBAPI.Modules;
using Scalar.AspNetCore;
using SharedLibrary.Configurations;
using Infrastructure.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
Console.WriteLine("KEY: " + tokenOptions!.SecurityKey);
Console.WriteLine("ISSUER: " + tokenOptions.Issuer);
Console.WriteLine("AUD: " + tokenOptions.Audience[0]);
Console.WriteLine("UTC NOW: " + DateTime.UtcNow);
 builder.Services.AddCustomJwtAuth(tokenOptions!);

builder.Services.AddRateLimiter(cfr =>
{
    cfr.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit=100;
        opt.Window=TimeSpan.FromSeconds(1);
        opt.QueueLimit=100;
        opt.QueueProcessingOrder=System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;

    });
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", x => {
        x.WithOrigins("https://localhost7232")//Authserver
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
});
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
//http leri https cevirir
app.UseHttpsRedirection();
app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine("API UTC NOW: " + DateTime.UtcNow);
    await next();
});

app.Use(async (context, next) =>
{
    Console.WriteLine("USER AUTH: " + context.User.Identity?.IsAuthenticated);
    await next();
});
app.Use(async (ctx, next) =>
{
    Console.WriteLine("MIDDLEWARE HIT");
    await next();
});
app.MapControllers().RequireRateLimiting("fixed");

app.MapBranch();
app.MapDevice();
app.Run();
