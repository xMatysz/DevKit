using Amazon.Extensions.Configuration.SystemsManager;
using Microsoft.Extensions.Configuration;

namespace DevKit.Api.Configuration;

public static class ConfigurationExtensions
{
    public static ConfigurationManager AddDevKitConfiguration(
        this ConfigurationManager configuration,
        string applicationName,
        string environmentName)
    {
        configuration.AddSystemsManager(cfg =>
        {
            cfg.Path = $"/{applicationName}/{environmentName}/";
            cfg.OnLoadException = OnException;
        })
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables();

        return configuration;
    }

    private static void OnException(SystemsManagerExceptionContext obj)
    {
        // TODO FIX ME
        obj.Ignore = true;
    }
}
