
using Infrastructure.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using NetWorkPassServer.Application;
using NetWorkPassServer.Infrastructure;
using NetWorkPassServer.Infrastructure.Hubs;
using NetWorkPassServer.WEBAPI.Modules;
using Scalar.AspNetCore;
using SharedLibrary.Configurations;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
builder.Services.ConfigureHttpJsonOptions(opt => { opt.SerializerOptions.PropertyNameCaseInsensitive=true;
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
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
        x.WithOrigins("https://localhost7232", "https://localhost:4200")//Authserver
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
    
    await next();
});

app.Use(async (context, next) =>
{
    
    await next();
});
app.Use(async (ctx, next) =>
{
    
    await next();
});
app.MapControllers().RequireRateLimiting("fixed");

app.MapBranch();
app.MapDevice();
app.MapHub<AlertHub>(
    "/hubs/alerts");
app.MapHub<MonitoringHub>(
    "/hubs/monitoring");
app.Run();
