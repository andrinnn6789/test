using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.IdentityServer.Plugin;
using IAG.Infrastructure.Settings;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication;

[ExcludeFromCodeCoverage]
public class UserDatabaseAuthenticationConfig : AuthenticationPluginConfig
{
    public string ConnectionString { get; set; }

    public UserDatabaseAuthenticationConfig()
    {
        Active = true;
        ConnectionString = $"Data Source={new SettingsFinder().GetSettingsPath()}\\IdentityServer.Users.db";
    }
}