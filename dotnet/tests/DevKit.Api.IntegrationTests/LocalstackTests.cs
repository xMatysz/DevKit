using Amazon;
using Amazon.SimpleSystemsManagement;
using DevKit.Example.Api;
using DevKit.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevKit.Api.IntegrationTests;

public class LocalstackTests : LocalstackFixture
{
    public LocalstackTests(DevKitWebAppFactory<IExampleApiMarker> factory)
        : base(factory)
    {
    }

    [Fact]
    public void ShouldSetLocalstackVariablesForAwsConfiguration()
    {
        var awsOptions = ServiceProvider.GetRequiredService<IConfiguration>().GetAWSOptions();

        awsOptions.DefaultClientConfig.ServiceURL.Should().Be(ServiceUrl);
        awsOptions.DefaultClientConfig.AuthenticationRegion.Should().Be(Region);
        awsOptions.Region.Should().Be(RegionEndpoint.GetBySystemName(Region));
    }

    [Fact]
    public void ShouldSetLocalstackVariablesForAwsService()
    {
        Factory.ConfigureService(sc => sc.AddAWSService<IAmazonSimpleSystemsManagement>());

        var awsService = ServiceProvider.GetRequiredService<IAmazonSimpleSystemsManagement>();

        awsService.Config.ServiceURL.Should().Be(ServiceUrl);
        awsService.Config.AuthenticationRegion.Should().Be(Region);
        awsService.Config.RegionEndpoint.Should().BeNull(because: "Service can have set only AuthRegion OR RegionEndpoint");
    }
}
