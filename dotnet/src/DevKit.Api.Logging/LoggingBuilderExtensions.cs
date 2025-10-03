using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DevKit.Api.Logging;

public static class LoggingBuilderExtensions
{
    private static readonly Action<LoggerConfiguration> ConsoleConfiguration =
        logConfig => logConfig
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .Enrich.FromLogContext()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

    private static Action<LoggerConfiguration, IConfiguration>
        ConsoleAndOtelConfiguration =>
        (logConfig, configuration) =>
        {
            ConsoleConfiguration(logConfig);

            logConfig.WriteTo.OpenTelemetry(_ => { }, configuration.GetValue<string>);
            logConfig.Enrich.With(new NewRelicLevelEnricher());
        };

    public static WebApplicationBuilder UseDevKitDefaultLogging(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? loggerConfigurationAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        loggerConfigurationAction ??= logConfig => ConsoleConfiguration(logConfig);

        builder.Host.UseSerilog((_, loggerConfiguration) => { loggerConfigurationAction(loggerConfiguration); });

        return builder;
    }

    public static WebApplicationBuilder UseDevKitLogging(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? loggerConfigurationAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfigurationAction ??= logConfig => ConsoleAndOtelConfiguration(
                logConfig,
                context.Configuration);

            loggerConfigurationAction(loggerConfiguration);
            loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
        });

        return builder;
    }

    // TODO: SGError -> use only one (HOST OR THIS)
    public static ILoggingBuilder AddDevKitLogger(
        this ILoggingBuilder builder,
        Action<LoggerConfiguration>? globalLoggerConfiguration = null)
    {
        var loggerConfiguration = new LoggerConfiguration();

        globalLoggerConfiguration ??= logConfig => ConsoleConfiguration(logConfig);
        globalLoggerConfiguration(loggerConfiguration);

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.AddSerilog();
        return builder;
    }
}
