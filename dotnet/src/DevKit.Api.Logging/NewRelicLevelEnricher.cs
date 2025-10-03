using Serilog.Core;
using Serilog.Events;

namespace DevKit.Api.Logging;

public sealed class NewRelicLevelEnricher : ILogEventEnricher
{
    private readonly Dictionary<LogEventLevel, string> _cachedLevel = new()
    {
        { LogEventLevel.Verbose, "Verbose" },
        { LogEventLevel.Debug, "Debug" },
        { LogEventLevel.Information, "Information" },
        { LogEventLevel.Warning, "Warning" },
        { LogEventLevel.Error, "Error" },
        { LogEventLevel.Fatal, "Fatal" },
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("level", _cachedLevel[logEvent.Level]));
    }
}
