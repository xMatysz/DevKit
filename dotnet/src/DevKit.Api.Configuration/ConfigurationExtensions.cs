using Amazon.Extensions.Configuration.SystemsManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevKit.Api.Configuration;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder UseDevKitConfiguration(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Configuration.AddDevKitConfiguration(
            builder.Environment.ApplicationName,
            builder.Environment.EnvironmentName);

        builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

        return builder;
    }

    public static ConfigurationManager AddDevKitConfiguration(
        this ConfigurationManager configuration,
        string applicationName,
        string environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration
            .AddEnvironmentVariables()
            .AddSystemsManager(cfg =>
            {
                cfg.Path = $"/{applicationName}/{environment}/";
                cfg.OnLoadException = OnException;
            })
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        return configuration;
    }

    private static void OnException(SystemsManagerExceptionContext obj)
    {
        // TODO FIX ME
        obj.Ignore = true;
    }
}
