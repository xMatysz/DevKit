using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DevKit.Base;

public sealed class DevKitOtelOptions
{
    private static readonly string _defaultInstanceId = Guid.NewGuid().ToString();
    private string? _serviceName;

    public string ServiceVersion { get; } = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;

    [ConfigurationKeyName("ApplicationName")]
    public required string ApplicationName { get; init; }
    public string ServiceName
    {
        get => _serviceName ??= ApplicationName;
        init => _serviceName = value;
    }

    public string InstanceId { get; init; } = _defaultInstanceId;
}
