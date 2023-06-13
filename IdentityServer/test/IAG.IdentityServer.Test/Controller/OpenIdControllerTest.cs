using System;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using IAG.IdentityServer.Controller;
using IAG.IdentityServer.Security;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Controller;

public class OpenIdControllerTest
{
    private readonly OpenIdController _controller;

    public OpenIdControllerTest()
    {
        var request = new CertificateRequest(
            new X500DistinguishedName("CN=TestCertificate"),
            RSA.Create(2048),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.MaxValue);

        var certificateManagerMock = new Mock<ICertificateManager>();
        certificateManagerMock.Setup(m => m.SigningCertificate).Returns(certificate);

        _controller = new OpenIdController(certificateManagerMock.Object)
        {
            ControllerContext =  new ControllerContext() { HttpContext = new DefaultHttpContext()}
        };
    }

    [Fact]
    public void GetOpenIdConnectConfigurationTest()
    {
        var result = _controller.GetOpenIdConnectConfiguration();

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.Issuer);
        Assert.NotEmpty(result.Value.TokenEndpoint);
        Assert.NotEmpty(result.Value.AuthorizationEndpoint);
        Assert.NotEmpty(result.Value.JwksUri);
        Assert.NotEmpty(result.Value.GrantTypesSupported);
        Assert.Contains(result.Value.GrantTypesSupported, x => x == OpenIdConnectGrantTypes.Password);
    }

    [Fact]
    public void GetKeyTest()
    {
        var result = _controller.GetKeys();

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.Keys);
    }

    [Fact]
    public void NotImplementedTest()
    {
        var result = _controller.NotImplemented();

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NotImplemented, Assert.IsAssignableFrom<StatusCodeResult>(result).StatusCode);
    }
}