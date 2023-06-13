using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace IAG.Infrastructure.IdentityServer.Authentication;

[ExcludeFromCodeCoverage]
public class AuthenticationToken : IAuthenticationToken
{
    public AuthenticationToken()
    {
        Claims = new List<Claim>();
    }

    public string Username { get; set; }

    public Guid? Tenant { get; set; }

    public string Email { get; set; }

    public string UserLanguage { get; set; }

    public bool UserShouldChangePassword { get; set; }

    public string Audience { get; set; }

    public TimeSpan? ValidFor { get; set; }

    public List<Claim> Claims { get; set; }

    public string RefreshToken { get; set; }
}