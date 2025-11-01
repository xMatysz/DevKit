using DevKit.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DevKit.Otel;

public static class ServiceCollectionExtensions
{
    public static IOpenTelemetryBuilder AddDevKitOtel(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ResourceBuilder>? resourceBuilder = null,
        Action<TracerProviderBuilder>? traceBuilder = null,
        Action<MeterProviderBuilder>? metricBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var otelOptions = configuration.Get<DevKitOtelOptions>()!;
        configuration["OTEL_SERVICE_NAME"] = otelOptions.ServiceName;
        configuration["OTEL_SERVICE_VERSION"] = otelOptions.ServiceVersion;
        configuration["OTEL_RESOURCE_ATTRIBUTES"] = $"service.instance.id={otelOptions.InstanceId}";

        var otelBuilder = services.AddOpenTelemetry()
            .ConfigureResource(resourceConfig =>
            {
                resourceConfig
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk();

                resourceBuilder?.Invoke(resourceConfig);
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

                traceBuilder?.Invoke(traceConfig);
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

                metricBuilder?.Invoke(metricConfig);
            });

        return otelBuilder;
    }
}
