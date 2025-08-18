using System.Reflection;
using DevKit.MediatR.Pipelines;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DevKit.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevKitMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        return services;
    }
}
