using AuthServer.Application.Abstractions;
using AuthServer.Application.Services;
using AuthServer.Infrastructure.Context;
using AuthServer.Infrastructure.Services;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SharedLibrary.Service;

namespace AuthServer.Infrastructure;
public static class ServiceRegistrar
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDbContext<AuthServerDbContext>((sp,options) =>
        {
            //var interceptor = sp.GetRequiredService<AuditInterceptor>();
            var connectionString = configuration.GetConnectionString("SqlServer");
            options.UseSqlServer(connectionString);
        });
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, EmailService>();
        //services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<AuthServerDbContext>());
        services.Scan(action => action.FromAssemblies(typeof(ServiceRegistrar).Assembly)
        .AddClasses(publicOnly: false)
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .AsImplementedInterfaces()
        .WithScopedLifetime());
        return services;
    }
}
