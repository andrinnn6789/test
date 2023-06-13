using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using IAG.IdentityServer.Security;
using IAG.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace IAG.IdentityServer.Controller;

public class OpenIdController : ControllerBase
{
    private readonly ICertificateManager _certificateManager;

    public OpenIdController(ICertificateManager certificateManager)
    {
        _certificateManager = certificateManager;
    }

    [AllowAnonymous]
    [HttpGet("/.well-known/openid-configuration")]
    public ActionResult<OpenIdConnectConfiguration> GetOpenIdConnectConfiguration()
    {
        var issuer = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var configuration = new OpenIdConnectConfiguration
        {
            Issuer = issuer,
            TokenEndpoint = $"{issuer}/{InfrastructureEndpoints.Auth}Realm/RequestToken",
            AuthorizationEndpoint = $"{issuer}/{InfrastructureEndpoints.Auth}OpenId/not-implemented",
            JwksUri = $"{issuer}/{InfrastructureEndpoints.Auth}OpenId/keys",
            JsonWebKeySet = GetSigningKeySet()
        };
        configuration.GrantTypesSupported.Add(OpenIdConnectGrantTypes.Password);
        configuration.GrantTypesSupported.Add(OpenIdConnectGrantTypes.RefreshToken);

        return configuration;
    }

    [AllowAnonymous]
    [HttpGet(InfrastructureEndpoints.Auth + "OpenId/keys")]
    public ActionResult<JsonWebKeySet> GetKeys()
    {
        return GetSigningKeySet();
    }

    [AllowAnonymous]
    [HttpGet(InfrastructureEndpoints.Auth + "OpenId/not-implemented")]
    public IActionResult NotImplemented()
    {
        return StatusCode((int)HttpStatusCode.NotImplemented);
    }

    private JsonWebKeySet GetSigningKeySet()
    {
        var rsaKey = _certificateManager.SigningCertificate.GetRSAPublicKey();
        var rsaParameters = rsaKey!.ExportParameters(false);
        var key = new JsonWebKey
        {
            // https://tools.ietf.org/html/rfc7517#section-4
            Kty = rsaKey.KeyExchangeAlgorithm,
            Use = "sig",
            Kid = _certificateManager.SigningCertificate.Thumbprint,
            X5t = _certificateManager.SigningCertificate.Thumbprint,

            // https://tools.ietf.org/html/rfc7517#appendix-B
            N = Convert.ToBase64String(rsaParameters.Modulus),
            E = Convert.ToBase64String(rsaParameters.Exponent),
        };

        var jwks = new JsonWebKeySet();
        jwks.Keys.Add(key);

        return jwks;

    }
}