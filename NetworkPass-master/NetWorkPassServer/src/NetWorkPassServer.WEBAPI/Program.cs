
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using NetWorkPassServer.Application;
using NetWorkPassServer.Infrastructure;
using NetWorkPassServer.Infrastructure.Hubs;
using NetWorkPassServer.WEBAPI.Modules;
using Scalar.AspNetCore;
using SharedLibrary.Configurations;
using SharedLibrary.Middlewares;
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
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); 
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", x => {
        x.WithOrigins("https://localhost:7232", "https://localhost:4200")//Authserver
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
});
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}
//http leri https cevirir
app.UseHttpsRedirection();
app.UseCors("cors");
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
//app.UseSerilogRequestLogging();
app.MapControllers().RequireRateLimiting("fixed");

app.MapBranch();
app.MapDevice();
app.RegisterDeviceHeartbeatRoutes();
app.RegisterAlertRoutes();
app.RegisterDashboardRoutes();
app.MapHub<DashboardHub>(
    
"/hubs/dashboard");
app.MapHub<AlertHub>(
    "/hubs/alerts");
app.MapHub<MonitoringHub>(
    "/hubs/monitoring");
app.Run();
