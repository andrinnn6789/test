using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.Infrastructure.IdentityServer.Model;

[ExcludeFromCodeCoverage]
public class RequestTokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType => "Bearer";

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("userLanguage")]
    public string UserLanguage { [UsedImplicitly] get; set; }

    [JsonProperty("userRole")]
    public string UserRole { [UsedImplicitly] get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("userShouldChangePassword")]
    public bool UserShouldChangePassword { [UsedImplicitly] get; set; }
}