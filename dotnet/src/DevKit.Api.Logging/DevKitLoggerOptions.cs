using Microsoft.Extensions.Configuration;

namespace DevKit.Api.Logging;

public class DevKitLoggerOptions
{
    [ConfigurationKeyName("DEVKIT_LOGGING_USE_CONFIGURATION")]
    public bool UseConfiguration { get; init; }
}
