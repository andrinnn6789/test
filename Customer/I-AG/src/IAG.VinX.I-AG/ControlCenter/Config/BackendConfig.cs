using System;

using IAG.IdentityServer.Plugin.UserDatabase.Authentication;

namespace IAG.VinX.IAG.ControlCenter.Config;

public class BackendConfig
{
    public string Realm { get; set; } = UserDatabaseAuthenticationPlugin.RealmName;

    public string Username { get; set; } = "$$backendUser$";

    public string Password { get; set; } = "$$backendPwd$";

    public string UrlAuth { get; set; } = "$$backendControl$";

    public string UrlControlCenter { get; set; } = "$$backendControl$";

    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(5);
}