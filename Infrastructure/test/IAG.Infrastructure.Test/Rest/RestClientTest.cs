using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using FluentAssertions;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Rest;

public class RestClientTest
{
    [Fact]
    public void ConstructorTest()
    {
        var baseUrlWithoutSlash = "http://test.i-ag.ch";
        var baseUrlWithSlash = baseUrlWithoutSlash + "/";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrlWithoutSlash;

        var client1 = new RestClient(mockConfig.Object);

        mockConfig.Object.BaseUrl = baseUrlWithSlash;
        var client2 = new RestClient(mockConfig.Object);

        Assert.NotNull(client1);
        Assert.NotNull(client2);
        Assert.Equal(baseUrlWithSlash, client1.BaseAddress?.ToString());
        Assert.Equal(baseUrlWithSlash, client2.BaseAddress?.ToString());
        Assert.NotNull(client1.DefaultRequestHeaders.Accept);
        Assert.NotNull(client2.DefaultRequestHeaders.Accept);
        Assert.NotEmpty(client1.DefaultRequestHeaders.Accept);
        Assert.NotEmpty(client2.DefaultRequestHeaders.Accept);
    }

    [Fact]
    public void ConstructorWithProxyTest()
    {
        var baseUrl = "http://test.i-ag.ch/";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl;

        var client = new RestClient(mockConfig.Object, new HttpProxy("localhost:8888"));

        Assert.NotNull(client);
        Assert.Equal(baseUrl, client.BaseAddress?.ToString());
        Assert.NotNull(client.DefaultRequestHeaders.Accept);
        Assert.NotEmpty(client.DefaultRequestHeaders.Accept);
    }

    [Fact]
    public void ConstructorWithAuthenticationTest()
    {
        var baseUrl = new Uri("http://test.i-ag.ch");
        var user = "user1";
        var password = "topSecret";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl.ToString();
        mockConfig.Object.Authentication = new BasicAuthentication() { User = user, Password = password };

        var client = new RestClient(mockConfig.Object);

        Assert.NotNull(client);
    }

    [Fact]
    public void ConstructorFailTest()
    {
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.Authentication = new BasicAuthentication();

        Assert.Throws<System.Exception>(() => new RestClient(mockConfig.Object));
    }

    [Fact]
    public void ConstructorOwnAcceptHeaderTest()
    {
        var baseUrl = new Uri("http://test.i-ag.ch");
        var acceptedType = "verySpecial/Type";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl.ToString();
        mockConfig.Object.HttpHeaders = new Dictionary<string, string>() { { "Accept", acceptedType } };

        var client = new RestClient(mockConfig.Object);

        Assert.NotNull(client);
        Assert.Equal(2, client.DefaultRequestHeaders.Accept.Count());
        Assert.Contains(client.DefaultRequestHeaders.Accept, p => p.MediaType == acceptedType);
        Assert.Contains(client.DefaultRequestHeaders.Accept, p => p.MediaType == ContentTypes.ApplicationJson);
    }

    [Fact]
    public void ConstructorOwnJsonAcceptHeaderTest()
    {
        var baseUrl = new Uri("http://test.i-ag.ch");
        var acceptedType = ContentTypes.ApplicationJson;
        var acceptedParam = "verySpecialType";
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl.ToString();
        mockConfig.Object.HttpHeaders = new Dictionary<string, string>() { { "Accept", acceptedType + ";" + acceptedParam } };

        var client = new RestClient(mockConfig.Object);
        var acceptHeader = client.DefaultRequestHeaders.Accept.FirstOrDefault(p => p.MediaType == acceptedType);

        Assert.NotNull(client);
        Assert.NotNull(acceptHeader);
        Assert.Single(client.DefaultRequestHeaders.Accept);
        Assert.Single(acceptHeader.Parameters);
        Assert.Equal(acceptedParam, acceptHeader.Parameters.First().Name);
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

    [Fact]
    public void PrepareRequestWithoutAuthenticationTest()
    {
        var baseUrl = new Uri("http://test.i-ag.ch");
        var mockConfig = new Mock<IHttpConfig>();
        mockConfig.SetupAllProperties();
        mockConfig.Object.BaseUrl = baseUrl.ToString();
        mockConfig.Object.Authentication = null;

        var client = new TestRestClient(mockConfig.Object);

        Assert.NotNull(client);
        client.CheckRequestPreparationWithoutAuthentication();
    }

    [Fact]
    public void PrepareRequestWithBasicAuthentication()
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
        client.CheckRequestPreparationWithBasicAuthentication();
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

        public void CheckRequestPreparationWithoutAuthentication()
        {
            var request = new JsonRestRequest(HttpMethod.Get, string.Empty);
            PrepareRequest(request);

            Assert.Null(request.Headers.Authorization);
        }

        public void CheckRequestPreparationWithBasicAuthentication()
        {
            var request = new JsonRestRequest(HttpMethod.Get, string.Empty);
            PrepareRequest(request);


            Assert.NotNull(request.Headers.Authorization);
            Assert.Equal("Basic", request.Headers.Authorization.Scheme);
            Assert.NotNull(request.Headers.Authorization.Parameter);
            Assert.NotEmpty(request.Headers.Authorization.Parameter);
        }
    }
}