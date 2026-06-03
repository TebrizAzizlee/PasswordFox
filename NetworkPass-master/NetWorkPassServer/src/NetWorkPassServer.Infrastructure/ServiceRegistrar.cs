using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Monitoring;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Infrastructure.Context;
using NetWorkPassServer.Infrastructure.Monitoring;
using NetWorkPassServer.Infrastructure.Monitoring.Strategies;
using NetWorkPassServer.Infrastructure.Services.Dashboard;
using NetWorkPassServer.Infrastructure.Services.Monitoring;
using Scrutor;


namespace NetWorkPassServer.Infrastructure;
public static class ServiceRegistrar
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDbContext<PasswordDbContext>(( options) =>
        {
            //var interceptor = sp.GetRequiredService<AuditInterceptor>();
            var connectionString = configuration.GetConnectionString("SqlServer");
            options.UseSqlServer(connectionString);
        });
        services.AddScoped<IDeviceMonitoringStrategy, PingMonitoringStrategy>();
        services.AddScoped<IDeviceMonitoringStrategyResolver, DeviceMonitoringStrategyResolver>(); 
        services.AddHostedService<DevicePollingBackgroundService>();
        services.AddScoped<IDashboardNotifier, DashboardNotifier>();


        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<PasswordDbContext>());
        services.AddScoped<IPasswordDbContext>(srv => srv.GetRequiredService<PasswordDbContext>());




        services.AddScoped<IDeviceMonitoringStrategy, PingMonitoringStrategy>();


        services
    .AddHostedService<VpnMonitoringWorker>();
        services.Scan(action=>action.FromAssemblies(typeof(ServiceRegistrar).Assembly)
        .AddClasses(publicOnly:false)
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .AsImplementedInterfaces()
        .WithScopedLifetime());    
        return services;
    }
}
