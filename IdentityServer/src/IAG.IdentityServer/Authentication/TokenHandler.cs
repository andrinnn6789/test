using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using IAG.IdentityServer.Configuration.Model.Config;
using IAG.IdentityServer.Resource;
using IAG.IdentityServer.Security;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.IdentityModel.Tokens;

namespace IAG.IdentityServer.Authentication;

public class TokenHandler : ITokenHandler
{
    private readonly ICertificateManager _certificateManager;
    private readonly TimeSpan _defaultExpirationTime;
    private readonly TimeSpan _tokenValidationClockSkew;


    public TokenHandler(ITokenGenerationConfig config, ICertificateManager certificateManager)
    {
        if (config.ExpirationTime <= TimeSpan.Zero)
        {
            throw new LocalizableException(ResourceIds.TokenHandlerConfigErrorExpiration);
        }
        if (config.TokenValidationClockSkew < TimeSpan.Zero)
        {
            throw new LocalizableException(ResourceIds.TokenHandlerConfigErrorClockSkew);
        }

        _certificateManager = certificateManager;
        _defaultExpirationTime = config.ExpirationTime;
        _tokenValidationClockSkew = config.TokenValidationClockSkew;
    }

    public bool? RequireHttpsMetadata => null;

    public string GetJwtToken(IAuthenticationToken authentication, string issuer)
    {
        var claims = new List<Claim>();

        if (authentication.Claims != null)
        {
            claims.AddRange(authentication.Claims);
        }

        AddMissingClaim(claims, ClaimTypes.Name, authentication.Username);
        AddMissingClaim(claims, ClaimTypes.Email, authentication.Email);
        AddMissingClaim(claims, "http://schemas.i-ag.ch/identity/claims/language", authentication.UserLanguage);

        var expirationTime = _defaultExpirationTime;
        if (authentication.ValidFor.HasValue && authentication.ValidFor.Value > TimeSpan.Zero)
        {
            expirationTime = authentication.ValidFor.Value;
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(expirationTime),
            SigningCredentials = new X509SigningCredentials(_certificateManager.SigningCertificate)
        };

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(jwt);
    }

    public void CheckJwtToken(string token, string issuer)
    {
        new JwtSecurityTokenHandler().ValidateToken(
            token,
            GetTokenValidationParameters(issuer),
            out _);
    }

    public TokenValidationParameters GetTokenValidationParameters(string issuer)
    {
        return new()
        {
            ClockSkew = _tokenValidationClockSkew,
            IssuerSigningKey = new X509SecurityKey(_certificateManager.SigningCertificate),
            ValidIssuer = issuer,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = true
        };
    }

    private void AddMissingClaim(List<Claim> claims, string claimType, string claimValue)
    {
        if (string.IsNullOrEmpty(claimValue))
        {
            return;
        }

        if (claims.Any(c => c.Type == claimType))
        {
            return;
        }

        claims.Add(new Claim(claimType, claimValue));
    }
}