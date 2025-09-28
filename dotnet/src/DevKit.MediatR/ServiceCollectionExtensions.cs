using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DevKit.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevKitMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddDevKitFluentValidation(assemblies);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            cfg.AddValidationBehaviour();
            cfg.AddUnitOfWork();
        });

        return services;
    }

    private static void AddDevKitFluentValidation(this IServiceCollection services, Assembly[] assemblies)
    {
        ValidatorOptions.Global.LanguageManager = new NameAndValueLanguageManager();
        ValidatorOptions.Global.MessageFormatterFactory = () => new NullMessageFormatter();
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name;

        services.AddValidatorsFromAssemblies(assemblies);
    }
}
