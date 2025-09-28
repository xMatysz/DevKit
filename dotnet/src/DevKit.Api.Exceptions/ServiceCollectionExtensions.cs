using Microsoft.Extensions.DependencyInjection;

namespace DevKit.Api.Exceptions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevKitExceptionHandlers(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
            };
        });

        services.AddExceptionHandler<ValidationExceptionHandler>();
        return services;
    }
}
