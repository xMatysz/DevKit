using Amazon.Extensions.Configuration.SystemsManager;
using Microsoft.Extensions.Configuration;

namespace DevKit.Api.Configuration;

public static class ConfigurationExtensions
{
    public static ConfigurationManager AddDevKitConfiguration(
        this ConfigurationManager configuration,
        string applicationName,
        string environment)
    {
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
