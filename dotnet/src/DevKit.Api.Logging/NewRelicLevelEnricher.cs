using Serilog.Core;
using Serilog.Events;

namespace DevKit.Api.Logging;

public sealed class NewRelicLevelEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("level", logEvent.Level.ToString()));
    }
}
