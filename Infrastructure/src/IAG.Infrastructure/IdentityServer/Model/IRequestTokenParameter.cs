using System;
using System.Collections.Generic;

namespace IAG.Infrastructure.IdentityServer.Model;

public interface IRequestTokenParameter
{
    string ClientId { get; set; }
    string ClientSecret { get; set; }
    string GrantType { get; set; }
    string Username { get; }
    string Password { get; }
    string RefreshToken { get; }
    Guid? TenantId { get; set; }
    List<string> Scopes { get; }
}