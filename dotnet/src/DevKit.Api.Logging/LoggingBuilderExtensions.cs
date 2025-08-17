using System.Globalization;
using DevKit.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace DevKit.Api.Logging;

public static class LoggingBuilderExtensions
{
    private const HttpLoggingFields DefaultHttpLogFields =
        HttpLoggingFields.ResponseStatusCode |
        HttpLoggingFields.Duration |
        HttpLoggingFields.RequestBody |
        HttpLoggingFields.RequestQuery |
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath;

    private static readonly Action<LoggerConfiguration> ConsoleConfiguration =
        logConfig => logConfig
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging", LogEventLevel.Information);

    private static Action<LoggerConfiguration, DevKitLoggerOptions, DevKitOtelOptions, IConfiguration>
        ConsoleAndOtelConfiguration =>
        (logConfig, devKitOptions, otelOptions, configuration) =>
        {
            ConsoleConfiguration(logConfig);

            logConfig.WriteTo.OpenTelemetry(cfg =>
            {
                cfg.Endpoint = otelOptions.Endpoint;
                cfg.Protocol = Enum.Parse<OtlpProtocol>(otelOptions.Protocol);
                cfg.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = otelOptions.ServiceName,
                    ["service.instance.id"] = otelOptions.InstanceId,
                    ["service.version"] = DevKitOtelOptions.ServiceVersion,
                };
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

        builder.Host.UseSerilog((_, loggerConfiguration) => { loggerConfigurationAction(loggerConfiguration); });

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

        builder.Services.AddHttpLogging(opt => { opt.LoggingFields = DefaultHttpLogFields; });

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
