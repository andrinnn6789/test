using System;
using System.Collections.Generic;

using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Identity;

namespace IAG.IdentityServer.Configuration.Model.Realm;

public interface IRealmConfig
{
    /// <summary>
    /// Unique realm. Preferably a descriptive key
    /// </summary>
    string Realm { get; }

    Guid AuthenticationPluginId { get; }

    IAuthenticationPluginConfig AuthenticationPluginConfig { get; set; }

    List<MailTemplateConfig> ResetPasswordMailTemplateConfig { get; }

    PasswordOptions PasswordPolicy { get; }
}