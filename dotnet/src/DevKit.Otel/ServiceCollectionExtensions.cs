using DevKit.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DevKit.Otel;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevKitOtel(this IServiceCollection services, IConfiguration configuration)
    {
        var otelOptions = configuration.Get<DevKitOtelOptions>()!;

        services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder
                    .AddService(otelOptions.ServiceName, serviceInstanceId: otelOptions.InstanceId)
                    .AddAttributes(
                    [
                        new KeyValuePair<string, object>("service.version", otelOptions.ServiceVersion)
                    ]);
            })
            .UseOtlpExporter(Enum.Parse<OtlpExportProtocol>(otelOptions.Protocol), new Uri(otelOptions.Endpoint))
            .WithTracing(traceConfig =>
            {
                traceConfig
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAWSInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql();
            })
            .WithMetrics(metricConfig =>
            {
                metricConfig
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAWSInstrumentation()
                    .AddNpgsqlInstrumentation();
            });

        return services;
    }
}
