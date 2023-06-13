using System;
using System.Collections.Generic;

using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Identity;

namespace IAG.IdentityServer.Test.Configuration.Mock;

public class TestRealmConfig : IRealmConfig
{
    public TestRealmConfig()
    {
        Realm = Name;
        AuthenticationPluginConfig = new TestPluginConfig();
        ResetPasswordMailTemplateConfig = new List<MailTemplateConfig>() { new() { Language = "de" } };
        PasswordPolicy = new PasswordOptions()
        {
            RequireDigit = false,
            RequireLowercase = false,
            RequireUppercase = false,
            RequireNonAlphanumeric = false
        };
    }

    public static string Name => "TestRealm";

    public string Realm { get; set; }

    public Guid AuthenticationPluginId => TestPlugin.Id;

    public IAuthenticationPluginConfig AuthenticationPluginConfig { get; set; }

    public List<MailTemplateConfig> ResetPasswordMailTemplateConfig { get; }

    public PasswordOptions PasswordPolicy { get; set; }
}