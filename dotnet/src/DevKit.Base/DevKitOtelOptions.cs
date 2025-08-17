using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DevKit.Base;

public class DevKitOtelOptions
{
    private static readonly string DefaultInstanceId = Guid.NewGuid().ToString();
    public string ServiceVersion => Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;

    [ConfigurationKeyName("ApplicationName")]
    public required string ApplicationName { get; init; }

    [ConfigurationKeyName("DEVKIT_OTEL_SERVICE_NAME")]
    public string? DevKitOtelName { get; init; }

    public string ServiceName => DevKitOtelName ?? ApplicationName;

    [ConfigurationKeyName("DEVKIT_OTEL_INSTANCEID")]
    public string InstanceId { get; init; } = DefaultInstanceId;

    [ConfigurationKeyName("DEVKIT_OTEL_ENDPOINT")]
    public required string Endpoint { get; init; }

    [ConfigurationKeyName("DEVKIT_OTEL_PROTOCOL")]
    public required string Protocol { get; init; }
}
