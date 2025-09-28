using System.Globalization;
using DevKit.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace DevKit.Api.Logging;

public static class LoggingBuilderExtensions
{
    private static readonly Action<LoggerConfiguration, IConfiguration> ConsoleConfiguration =
        (logConfig, configuration) => logConfig
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .Enrich.FromLogContext()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

    private static Action<LoggerConfiguration, DevKitLoggerOptions, DevKitOtelOptions, IConfiguration>
        ConsoleAndOtelConfiguration =>
        (logConfig, devKitOptions, otelOptions, configuration) =>
        {
            ConsoleConfiguration(logConfig, configuration);

            logConfig.WriteTo.OpenTelemetry(_ => new BatchedOpenTelemetrySinkOptions(), configuration.GetValue<string>);
            logConfig.Enrich.With(new NewRelicLevelEnricher());
        };

    public static WebApplicationBuilder UseDevKitDefaultLogging(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? loggerConfigurationAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        loggerConfigurationAction ??= logConfig => ConsoleConfiguration(
            logConfig,
            builder.Configuration);

        builder.Host.UseSerilog((_, loggerConfiguration) => { loggerConfigurationAction(loggerConfiguration); });

        return builder;
    }

    public static WebApplicationBuilder UseDevKitLogging(
        this WebApplicationBuilder builder,
        Action<DevKitLoggerOptions>? devKitLoggerOptionsAction = null,
        Action<LoggerConfiguration>? loggerConfigurationAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Logging.ClearProviders();

        var devKitLoggerOptions = builder.Configuration.Get<DevKitLoggerOptions>()!;
        devKitLoggerOptionsAction?.Invoke(devKitLoggerOptions);

        var otelOptions = builder.Configuration.Get<DevKitOtelOptions>()!;

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfigurationAction ??= logConfig => ConsoleAndOtelConfiguration(
                logConfig,
                devKitLoggerOptions,
                otelOptions,
                context.Configuration);

            loggerConfigurationAction(loggerConfiguration);
        });

        return builder;
    }

    // TODO: SGError -> use only one (HOST OR THIS)
    public static ILoggingBuilder AddDevKitLogger(
        this ILoggingBuilder builder,
        IConfiguration configuration,
        Action<LoggerConfiguration>? globalLoggerConfiguration = null)
    {
        var loggerConfiguration = new LoggerConfiguration();

        globalLoggerConfiguration ??= logConfig => ConsoleConfiguration(logConfig, configuration);
        globalLoggerConfiguration(loggerConfiguration);

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.AddSerilog();
        return builder;
    }
}
