
namespace IAG.Infrastructure.IdentityServer.Authentication;

public static class GrantTypes
{
    public static readonly string AuthorizationCode = "authorization_code";
    public static readonly string ClientCredentials = "client_credentials";
    public static readonly string Password = "password";
    public static readonly string RefreshToken = "refresh_token";
    public static readonly string DeviceCode = "urn:ietf:params:oauth:grant-type:device_code";
}