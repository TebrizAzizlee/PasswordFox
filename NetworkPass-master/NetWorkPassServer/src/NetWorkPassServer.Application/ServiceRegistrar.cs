using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using SharedLibrary.Behaviors;
using TS.MediatR;

namespace NetWorkPassServer.Application;
public static class ServiceRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfr =>
        {
            cfr.RegisterServicesFromAssembly(typeof(ServiceRegistrar).Assembly);
            cfr.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(typeof(ServiceRegistrar).Assembly);
        return services;
    }
}
