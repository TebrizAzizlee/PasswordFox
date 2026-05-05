using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetWorkPassServer.Infrastructure.Context;
using Scrutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<PasswordDbContext>());
        services.Scan(action=>action.FromAssemblies(typeof(ServiceRegistrar).Assembly)
        .AddClasses(publicOnly:false)
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .AsImplementedInterfaces()
        .WithScopedLifetime());    
        return services;
    }
}
