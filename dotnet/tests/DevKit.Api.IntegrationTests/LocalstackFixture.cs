using DevKit.Example.Api;
using DevKit.Tests.Integration.Shared;
using Xunit;

namespace DevKit.Api.IntegrationTests;

public class LocalstackFixture : IClassFixture<DevKitWebAppFactory<IExampleApiMarker>>
{
    protected const string ServiceUrl = "http://10.10.10.10:4655/";
    protected const string Region = "eu-central-1";

    protected DevKitWebAppFactory<IExampleApiMarker> Factory { get; }
    protected IServiceProvider ServiceProvider => Factory.Services;

    public LocalstackFixture(DevKitWebAppFactory<IExampleApiMarker> factory)
    {
        Factory = factory;

        SetLocalstackVariables();
    }

    private void SetLocalstackVariables()
    {
        Factory.AddConfiguration(
        [
            new KeyValuePair<string, string?>("AWS:ServiceUrl", ServiceUrl),
            new KeyValuePair<string, string?>("AWS:AuthenticationRegion", Region),
            new KeyValuePair<string, string?>("AWS:Region", Region),
        ]);
    }
}
