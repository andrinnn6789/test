using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration.Model.Config;
using IAG.IdentityServer.Security;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.IdentityModel.Tokens;

using Moq;

using Xunit;

using TokenHandler = IAG.IdentityServer.Authentication.TokenHandler;

namespace IAG.IdentityServer.Test.Authentication;

public class TokenHandlerTest
{
    private readonly ITokenGenerationConfig _defaultConfig;
    private readonly ICertificateManager _certificateManager;

    public TokenHandlerTest()
    {
        _defaultConfig = CreateTokenGenerationConfig(TimeSpan.FromHours(7), TimeSpan.FromMinutes(1));

        var request = new CertificateRequest(
            new X500DistinguishedName("CN=TestCertificate"),
            RSA.Create(2048),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.MaxValue);

        var certificateManagerMock = new Mock<ICertificateManager>();
        certificateManagerMock.Setup(m => m.SigningCertificate).Returns(certificate);

        _certificateManager = certificateManagerMock.Object;
    }

    [Fact]
    public void ConstructorWithFullConfigTest()
    {
        var handler = new TokenHandler(_defaultConfig, _certificateManager);

        Assert.NotNull(handler);
        Assert.Null(handler.RequireHttpsMetadata);
    }

    [Fact]
    public void ConstructorInvalidExpirationTimeConfigTest()
    {
        var configInvalidExpiration = CreateTokenGenerationConfig(TimeSpan.Zero, TimeSpan.FromMinutes(1));
        var configInvalidClockSkew = CreateTokenGenerationConfig(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(-1));

        Assert.Throws<LocalizableException>(() => new TokenHandler(configInvalidExpiration, _certificateManager));
        Assert.Throws<LocalizableException>(() => new TokenHandler(configInvalidClockSkew, _certificateManager));
    }

    [Fact]
    public void GetJwtTokenTest()
    {
        var claims = new List<Claim>
        {
            new("dummy", "foobar"),
            new(ClaimTypes.Name, "mustermann")
        };
        var authentication = new Mock<IAuthenticationToken>();
        authentication.SetupGet(m => m.Username).Returns("mustermann");
        authentication.SetupGet(m => m.Email).Returns("muster@mann.ch");
        authentication.SetupGet(m => m.Claims).Returns(claims);

        var handler = new TokenHandler(_defaultConfig, _certificateManager);
        var token = handler.GetJwtToken(authentication.Object, "TestIssuer");
        var jwtToken = new JwtSecurityToken(token);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Contains(jwtToken.Claims, (claim) => claim.Type == JwtRegisteredClaimNames.Name && claim.Value == "mustermann");
        Assert.Contains(jwtToken.Claims, (claim) => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == "muster@mann.ch");
        Assert.Contains(jwtToken.Claims, (claim) => claim.Type == "dummy" && claim.Value == "foobar");
    }

    [Fact]
    public void CheckJwtTokenTest()
    {
        var authentication = new Mock<IAuthenticationToken>();
        authentication.SetupGet(m => m.Username).Returns("mustermann");

        var handler = new TokenHandler(_defaultConfig, _certificateManager);
        var token = handler.GetJwtToken(authentication.Object, "TestIssuer");

        handler.CheckJwtToken(token, "TestIssuer");
    }

    [Fact]
    public async Task CheckInvalidJwtTokenTest()
    {
        var config = CreateTokenGenerationConfig(TimeSpan.FromHours(7), TimeSpan.Zero);
        var handler = new TokenHandler(config, _certificateManager);
        var authTokenWithShortValidity = new Mock<IAuthenticationToken>();
        authTokenWithShortValidity.Setup(m => m.ValidFor).Returns(TimeSpan.FromMilliseconds(100));

        var emptyToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken());
        var testIssuerToken = handler.GetJwtToken(new Mock<IAuthenticationToken>().Object, "TestIssuer");
        var testShortValidToken = handler.GetJwtToken(authTokenWithShortValidity.Object, "TestIssuer");

        await Task.Delay(500);

        Assert.Throws<ArgumentException>(() => { handler.CheckJwtToken("NotAToken.JustASillyText", "Irrelevant"); });
        Assert.Throws<SecurityTokenInvalidSignatureException>(() => { handler.CheckJwtToken(emptyToken, "Irrelevant"); });
        Assert.Throws<SecurityTokenInvalidIssuerException>(() => { handler.CheckJwtToken(testIssuerToken, "OtherIssuer"); });
        Assert.Throws<SecurityTokenExpiredException>(() => { handler.CheckJwtToken(testShortValidToken, "TestIssuer"); });
    }

    [Fact]
    public async Task CheckExpiredJwtTokenTest()
    {
        var config = CreateTokenGenerationConfig(TimeSpan.FromMilliseconds(100), TimeSpan.Zero);
        var handler = new TokenHandler(config, _certificateManager);
        var token = handler.GetJwtToken(new Mock<IAuthenticationToken>().Object, "TestIssuer");

        await Task.Delay(500);

        Assert.Throws<SecurityTokenExpiredException>(() => { handler.CheckJwtToken(token, "TestIssuer"); });
    }


    private ITokenGenerationConfig CreateTokenGenerationConfig(TimeSpan expirationTime, TimeSpan clockSkew)
    {
        var configMock = new Mock<ITokenGenerationConfig>();
        configMock.SetupGet(m => m.ExpirationTime).Returns(expirationTime);
        configMock.SetupGet(m => m.TokenValidationClockSkew).Returns(clockSkew);

        return configMock.Object;
    }
}