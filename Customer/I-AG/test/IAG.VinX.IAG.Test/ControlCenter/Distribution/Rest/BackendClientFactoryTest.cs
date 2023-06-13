using System;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.Rest;

public class BackendClientFactoryTest
{
    private readonly IControlCenterTokenRequest _requestToken;

    public BackendClientFactoryTest()
    {
        var requestTokenMock = new Mock<IControlCenterTokenRequest>();
        requestTokenMock.Setup(m => m.GetConfig(It.IsAny<BackendConfig>(), It.IsAny<string>(), It.IsAny<IRequestResponseLogger>()))
            .Returns(new HttpConfig { BaseUrl = "http://baseurlfromtokenrequest", Authentication = new BearerAuthentication("token")});

        _requestToken = requestTokenMock.Object;
    }

    [Fact]
    public void CreateClientWithoutAuthenticationTest()
    {
        var backendConfig = new BackendConfig { UrlControlCenter = "http://cchost", Username = null};
        var clientFactory = new BackendClientFactory(backendConfig, _requestToken, null);
        var client = clientFactory.CreateRestClient<TestRestClient>("endpoint");

        Assert.NotNull(client);
        Assert.NotNull(client.Config);
        Assert.Null(client.Config.Authentication);
        Assert.Equal("http://cchost/endpoint", client.Config.BaseUrl);
        Assert.Null(client.Logger);
    }

    [Fact]
    public void CreateClientWithAuthenticationTest()
    {
        var backendConfig = new BackendConfig { UrlControlCenter = "http://cchost", Username = "TestUser"};
        var clientFactory = new BackendClientFactory(backendConfig, _requestToken, new Mock<ILogger>().Object);
        var client = clientFactory.CreateRestClient<TestRestClient>("endpoint");

        Assert.NotNull(client);
        Assert.NotNull(client.Config);
        Assert.NotNull(client.Config.Authentication);
        Assert.Equal("token", Assert.IsType<BearerAuthentication>(client.Config.Authentication).Token);
        Assert.Equal("http://baseurlfromtokenrequest", client.Config.BaseUrl);
        Assert.NotNull(client.Logger);
    }

    [Fact]
    public void AuthenticationFailsTest()
    {
        var backendConfig = new BackendConfig { UrlControlCenter = "http://cchost", Username = "TestUser" };
        var requestTokenMock = new Mock<IControlCenterTokenRequest>();
        requestTokenMock.Setup(m => m.GetConfig(It.IsAny<BackendConfig>(), It.IsAny<string>(), It.IsAny<IRequestResponseLogger>()))
            .Returns(() => throw new Exception("Error"));

        var clientFactory = new BackendClientFactory(backendConfig, requestTokenMock.Object, new Mock<ILogger>().Object);

        Assert.Throws<LocalizableException>(() => clientFactory.CreateRestClient<TestRestClient>("endpoint"));
    }


    private class TestRestClient : RestClient
    {
        public IHttpConfig Config { get; }
        public new IRequestResponseLogger Logger => base.Logger;

        [UsedImplicitly]
        public TestRestClient(IHttpConfig config, IRequestResponseLogger logger = null) : base(config, logger)
        {
            Config = config;
        }
    }
}