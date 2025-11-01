using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevKit.Tests.Integration.Shared;

public sealed class DevKitWebAppFactory<T> : WebApplicationFactory<T>
    where T : class
{
    private readonly List<KeyValuePair<string, string?>> _testConfiguration = [];
    private Action<IServiceCollection>? _configureServices;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            _configureServices?.Invoke(services);
        });

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(_testConfiguration);
        });
    }

    public void ConfigureService(Action<IServiceCollection> action)
    {
        _configureServices += action;
    }

    public void AddConfiguration(KeyValuePair<string, string?>[] configurations)
    {
        _testConfiguration.AddRange(configurations);
    }

    public override ValueTask DisposeAsync()
    {
        _configureServices = null;
        _testConfiguration.Clear();

        return base.DisposeAsync();
    }
}
