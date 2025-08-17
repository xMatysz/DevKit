using Microsoft.Extensions.Configuration;
using Serilog.Sinks.OpenTelemetry;

namespace DevKit.Api.Logging;

public class DevKitLoggerOptions
{
    [ConfigurationKeyName("DEVKIT_LOGGING_OTEL_ENDPOINT")]
    public string OtelEndpoint { get; set; } = string.Empty;

    [ConfigurationKeyName("DEVKIT_LOGGING_OTEL_PROTOCOL")]
    public OtlpProtocol OtelProtocol { get; set; }

    [ConfigurationKeyName("DEVKIT_LOGGING_OTEL_RESOURCE_ATTRIBUTES")]
    public Dictionary<string, object> ResourceAttributes { get; } = [];

    [ConfigurationKeyName("DEVKIT_LOGGING_USE_CONFIGURATION")]
    public bool UseConfiguration { get; set; }
}
