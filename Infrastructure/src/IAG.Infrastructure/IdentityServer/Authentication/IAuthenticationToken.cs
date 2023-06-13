using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IAG.Infrastructure.IdentityServer.Authentication;

public interface IAuthenticationToken
{
    string Username { get; set; }

    Guid? Tenant { get; set; }

    string UserLanguage { get; }

    string Email { get; }

    bool UserShouldChangePassword { get; }

    TimeSpan? ValidFor { get; set; }

    List<Claim> Claims { get; }

    string RefreshToken { get; set; }
}