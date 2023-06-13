using System;

using FluentAssertions;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;
using IAG.Infrastructure.Rest.Authentication;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Rest.Atlas;

public class AtlasClientTest
{
    [Fact]
    public void ConstructorTest()
    {
        var baseUrl = "http://test.i-ag.ch/";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl;

        var client = new AtlasClient(mockConfig.Object);
        var clientWithProxy = new AtlasClient(mockConfig.Object, new HttpProxy("localhost:8888"));

        Assert.NotNull(client);
        Assert.NotNull(clientWithProxy);
        Assert.NotNull(client.AtlasConfiguration);
        Assert.Equal(baseUrl, client.BaseAddress?.ToString());
        Assert.Equal(baseUrl, clientWithProxy.BaseAddress?.ToString());
    }

    [Fact]
    public void ConfigurationTest()
    {
        var baseUrl = new Uri("http://test.i-ag.ch");
        var user = "user1";
        var password = "topSecret";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl.ToString();
        mockConfig.Object.Authentication = new BasicAuthentication() { User = user, Password = password };

        var client = new TestRestClient(mockConfig.Object);

        Assert.NotNull(client);
        client.CheckConfig(mockConfig.Object);
    }

    private class TestRestClient : RestClient
    {
        public TestRestClient(IHttpConfig config)
            : base(config)
        {
        }

        public void CheckConfig(IHttpConfig config)
        {
            Configuration.Should().BeEquivalentTo(config);
        }
    }
}