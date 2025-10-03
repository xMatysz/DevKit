using DevKit.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DevKit.Otel;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevKitOtel(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var otelOptions = configuration.Get<DevKitOtelOptions>()!;
        configuration["OTEL_SERVICE_NAME"] = otelOptions.ServiceName;
        configuration["OTEL_SERVICE_VERSION"] = otelOptions.ServiceVersion;
        configuration["OTEL_RESOURCE_ATTRIBUTES"] = $"service.instance.id={otelOptions.InstanceId}";

        services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk();
            })
            .WithTracing(traceConfig =>
            {
                traceConfig
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAWSInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql();

                traceConfig.AddOtlpExporter();
            })
            .WithMetrics(metricConfig =>
            {
                metricConfig
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAWSInstrumentation()
                    .AddNpgsqlInstrumentation();

                metricConfig.AddOtlpExporter();
            });

        return services;
    }
}
