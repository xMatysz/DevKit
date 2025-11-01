using System.Diagnostics;
using DevKit.Api.Exceptions.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DevKit.Api.Exceptions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevKitExceptionHandlers(
        this IServiceCollection services,
        Action<ProblemDetailsOptions>? problemDetailsOptions = null)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("traceId", Activity.Current?.TraceId.ToString());
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };

            problemDetailsOptions?.Invoke(options);
        });

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}
