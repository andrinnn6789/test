using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.Startup.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Authorization;
// TODO: Tests für Remote (?)

[Collection("InfrastructureController")]
public class AuthenticationModeOffTest : AuthenticationModeTest
{
    public AuthenticationModeOffTest() : base(AuthenticationMode.Off)
    {
    }
        
    [Fact]
    public async Task GetAsAnonymousTest()
    {
        var httpClient = GetClient();

        var anonymousResponse = await httpClient.GetAsync("Test/AnonymousTest");
        var anonymousResult = await anonymousResponse.Content.ReadAsStringAsync();
        var authorizedResponse = await httpClient.GetAsync("Test/AuthorizedTest");
        var authorizedResult = await authorizedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, authorizedResponse.StatusCode);
        Assert.Equal("Anonymous", anonymousResult);
        Assert.Equal("Authorized", authorizedResult);
    }

    [Fact]
    public async Task GetAsAuthorizedTest()
    {
        var httpClient = GetClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ValidJwt");

        var anonymousResponse = await httpClient.GetAsync("Test/AnonymousTest");
        var anonymousResult = await anonymousResponse.Content.ReadAsStringAsync();
        var authorizedResponse = await httpClient.GetAsync("Test/AuthorizedTest");
        var authorizedResult = await authorizedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, authorizedResponse.StatusCode);
        Assert.Equal("Anonymous", anonymousResult);
        Assert.Equal("Authorized", authorizedResult);
    }
}

[Collection("InfrastructureController")]
public class AuthenticationModeRemoteTest : AuthenticationModeTest
{
    public AuthenticationModeRemoteTest() : base(AuthenticationMode.Remote)
    {
    }

    [Fact]
    public async Task GetAsAnonymousTest()
    {
        var httpClient = GetClient();

        var anonymousResponse = await httpClient.GetAsync("Test/AnonymousTest");
        var anonymousResult = await anonymousResponse.Content.ReadAsStringAsync();
        var authorizedResponse = await httpClient.GetAsync("Test/AuthorizedTest");
        var authorizedResult = await authorizedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, authorizedResponse.StatusCode);
        Assert.Equal("Anonymous", anonymousResult);
        Assert.Equal("Authorized", authorizedResult);
    }

    [Fact]
    public async Task GetAsAuthorizedTest()
    {
        var httpClient = GetClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ValidJwt");

        var anonymousResponse = await httpClient.GetAsync("Test/AnonymousTest");
        var anonymousResult = await anonymousResponse.Content.ReadAsStringAsync();
        var authorizedResponse = await httpClient.GetAsync("Test/AuthorizedTest");
        var authorizedResult = await authorizedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, authorizedResponse.StatusCode);
        Assert.Equal("Anonymous", anonymousResult);
        Assert.Equal("Authorized", authorizedResult);
    }
}

[Collection("InfrastructureController")]
public class AuthenticationModeAllTest : AuthenticationModeTest
{
    public AuthenticationModeAllTest() : base(AuthenticationMode.All)
    {
    }

    [Fact]
    public async Task GetAsAnonymousTest()
    {
        var httpClient = GetClient();
        var anonymousResponse = await httpClient.GetAsync("Test/AnonymousTest");
        var anonymousResult = await anonymousResponse.Content.ReadAsStringAsync();
        var authorizedResponse = await httpClient.GetAsync("Test/AuthorizedTest");

        Assert.Equal(HttpStatusCode.OK, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, authorizedResponse.StatusCode);
        Assert.Equal("Anonymous", anonymousResult);
    }

    [Fact]
    public async Task GetAsAuthorizedTest()
    {
        var httpClient = GetClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ValidJwt");

        var anonymousResponse = await httpClient.GetAsync("Test/AnonymousTest");
        var anonymousResult = await anonymousResponse.Content.ReadAsStringAsync();
        var authorizedResponse = await httpClient.GetAsync("Test/AuthorizedTest");
        var authorizedResult = await authorizedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, authorizedResponse.StatusCode);
        Assert.Equal("Anonymous", anonymousResult);
        Assert.Equal("Authorized", authorizedResult);
    }
}

public abstract class AuthenticationModeTest : IDisposable
{
    private readonly IHost _server;

    protected AuthenticationModeTest(AuthenticationMode authenticationMode)
    {
        var claimPublisherMock = new Mock<IClaimPublisher>();
        claimPublisherMock.Setup(m => m.PublishClaimsAsync(It.IsAny<IEnumerable<ClaimDefinition>>())).Returns(Task.CompletedTask);

        var tokenHandlerMock = new Mock<ITokenHandler>();
        tokenHandlerMock.Setup(m => m.GetTokenValidationParameters(It.IsAny<string>()))
            .Returns(new TokenValidationParameters());
        tokenHandlerMock.SetupGet(m => m.RequireHttpsMetadata).Returns(false);

        Startup.Startup.CleanConfig();
        var appSettings = Path.Combine(Environment.CurrentDirectory, "Authorization",
            $"appsettings.{authenticationMode}.json");
        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost
                    .UseTestServer()
                    .UseEnvironment("Test")
                    .ConfigureLogging(factory => { factory.AddDebug(); })
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(claimPublisherMock.Object);
                        services.AddSingleton(tokenHandlerMock.Object);
                        services.Replace(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, TestJwtBearerPostConfigureOptions>());
                    })
                    .UseStartup<Startup.Startup>()
                    .UseConfiguration(Startup.Startup.BuildConfig(new[] { $"appsettings={appSettings}" }));
            });

        _server = hostBuilder.Start();

    }

    protected HttpClient GetClient()
        => _server.GetTestClient();

    public void Dispose()
    {
        _server.Dispose();
        Startup.Startup.CleanConfig();
    }
}