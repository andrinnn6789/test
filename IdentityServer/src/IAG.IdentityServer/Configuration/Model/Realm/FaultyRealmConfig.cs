using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Identity;

namespace IAG.IdentityServer.Configuration.Model.Realm;

[ExcludeFromCodeCoverage]
public class FaultyRealmConfig : IRealmConfig
{
    public string Realm { get; set; }

    public Guid AuthenticationPluginId { get; set; }

    public IAuthenticationPluginConfig AuthenticationPluginConfig
    {
        get => null;
        set { }
    }

    public List<MailTemplateConfig> ResetPasswordMailTemplateConfig => null;

    public PasswordOptions PasswordPolicy => null;

    public string ErrorMessage { get; set; }
}