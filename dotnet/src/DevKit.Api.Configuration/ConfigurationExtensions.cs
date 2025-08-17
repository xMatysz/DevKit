using Microsoft.Extensions.Configuration;

namespace DevKit.Api.Configuration;

public static class ConfigurationExtensions
{
    public static ConfigurationManager AddDevKitConfiguration(this ConfigurationManager configuration, string environmentName)
    {
        configuration.AddSystemsManager(cfg =>
        {
            cfg.Path = $"/DevKit/Test/";
        })
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables();

        return configuration;
    }
}
