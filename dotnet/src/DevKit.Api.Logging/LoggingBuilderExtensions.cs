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
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging", LogEventLevel.Information);

    private static Action<LoggerConfiguration, DevKitLoggerOptions, IConfiguration> ConsoleAndOtelConfiguration =>
        (logConfig, devKitOptions, configuration) =>
        {
            ConsoleConfiguration(logConfig);

            logConfig.WriteTo.OpenTelemetry(cfg =>
            {
                cfg.Endpoint = devKitOptions.OtelEndpoint;
                cfg.Protocol = devKitOptions.OtelProtocol;
                cfg.ResourceAttributes = devKitOptions.ResourceAttributes;
            });

            if (devKitOptions.UseConfiguration)
            {
                logConfig.ReadFrom.Configuration(configuration);
            }
        };

    public static WebApplicationBuilder UseDevKitDefaultLogging(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? loggerConfigurationAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        loggerConfigurationAction ??= ConsoleConfiguration;

        builder.Host.UseSerilog((_, loggerConfiguration) =>
        {
            loggerConfigurationAction(loggerConfiguration);
        });

        return builder;
    }

    public static WebApplicationBuilder UseDevKitLogging(
        this WebApplicationBuilder builder,
        Action<DevKitLoggerOptions>? devKitLoggerOptionsAction = null,
        Action<LoggerConfiguration>? loggerConfigurationAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var devKitLoggerOptions = builder.Configuration.Get<DevKitLoggerOptions>()!;
        devKitLoggerOptionsAction?.Invoke(devKitLoggerOptions);

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfigurationAction ??= logConfig => ConsoleAndOtelConfiguration(
                logConfig,
                devKitLoggerOptions,
                context.Configuration);

            loggerConfigurationAction(loggerConfiguration);
        });

        return builder;
    }

    // TODO: SGError -> use only one
    public static ILoggingBuilder AddDevKitLogger(
        this ILoggingBuilder builder,
        Action<LoggerConfiguration>? globalLoggerConfiguration = null,
        IConfiguration? configuration = null)
    {
        var loggerConfiguration = new LoggerConfiguration();

        globalLoggerConfiguration ??= ConsoleConfiguration;
        globalLoggerConfiguration(loggerConfiguration);

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.AddSerilog();
        return builder;
    }
}
