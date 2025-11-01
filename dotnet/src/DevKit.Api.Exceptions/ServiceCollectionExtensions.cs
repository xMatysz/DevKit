using System.Diagnostics;
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
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("traceId", Activity.Current?.TraceId.ToString());
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<ValidationExceptionHandler>();
        return services;
    }
}
