using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace IAG.Infrastructure.IdentityServer.Model;

[ExcludeFromCodeCoverage]
public class SimpleRequestTokenParameter
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("tenantId")]
    public Guid? TenantId { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    public RequestTokenParameter ToRequestTokenParameter()
    {
        return !string.IsNullOrEmpty(RefreshToken)
            ? RequestTokenParameter.ForRefreshGrant(RefreshToken)
            : RequestTokenParameter.ForPasswordGrant(Username, Password, TenantId);
    }
}