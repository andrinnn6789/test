using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using IAG.Infrastructure.IdentityServer.Authentication;

using Newtonsoft.Json;

namespace IAG.Infrastructure.IdentityServer.Model;

public class RequestTokenParameter : IRequestTokenParameter
{
    private static readonly string AcrValueKeyTenant = "tenant:";

    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    [JsonProperty("grant_type")]
    public string GrantType { get; set; } = GrantTypes.Password;

    [JsonProperty("scope")]
    public string ScopeSetter {
        set => Scopes = value?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
    }

    [JsonProperty("acr_values")]
    public string AcrValuesSetter {
        set
        {
            var values = value?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var tenant = values?.FirstOrDefault(v => v.StartsWith(AcrValueKeyTenant))?.Substring(AcrValueKeyTenant.Length);

            if ((tenant != null) && Guid.TryParse(tenant, out var tenantId))
            {
                TenantId = tenantId;
            }
        }
    }

    [JsonIgnore]
    public string Realm
    {
        get => ClientId;
        set => ClientId = value;
    }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("tenantId")]
    public Guid? TenantId { get; set; }

    [JsonProperty("scopes")]
    public List<string> Scopes { get; private set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    public RequestTokenParameter ForRealm(string realm)
    {
        Realm = realm;
        return this;
    }

    public static RequestTokenParameter ForPasswordGrant(string username, string password, Guid? tenantId = null)
        => new ()
        {
            GrantType = GrantTypes.Password,
            Username = username,
            Password = password,
            TenantId = tenantId,
            Scopes = new List<string>()
        };

    public static RequestTokenParameter ForRefreshGrant(string refreshToken)
        => new()
        {
            GrantType = GrantTypes.RefreshToken,
            RefreshToken= refreshToken
        };

    #region OAuth2 properties may used later...

    [ExcludeFromCodeCoverage]
    [JsonProperty("client_secret")]
    public string ClientSecret { get; set; }

    [ExcludeFromCodeCoverage]
    [JsonProperty("redirect_uri")]
    public string RedirectUri { get; set; }

    [ExcludeFromCodeCoverage]
    [JsonProperty("code")]
    public string Code { get; set; }

    [ExcludeFromCodeCoverage]
    [JsonProperty("code_verifier")]
    public string CodeVerifier { get; set; }

    [ExcludeFromCodeCoverage]
    [JsonProperty("device_code")]
    public string DeviceCode { get; set; }

    #endregion
}