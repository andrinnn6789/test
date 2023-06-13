using System;
using System.Collections.Generic;

using IAG.IdentityServer.Resource;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Identity;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAG.IdentityServer.Configuration.Model.Realm;

public class RealmConfig : IRealmConfig
{
    public string Realm { get; set; }

    public Guid AuthenticationPluginId { get; set; }

    [JsonIgnore]
    public IAuthenticationPluginConfig AuthenticationPluginConfig { get; set; }

    [JsonProperty("AuthenticationPluginConfig")]
    public JObject AuthenticationPluginConfigJObject { get; set; }

    public List<MailTemplateConfig> ResetPasswordMailTemplateConfig { get; set; }

    public PasswordOptions PasswordPolicy { get; set; }

    public void SetPluginConfig(IPluginCatalogue pluginCatalogue)
    {
        var plugin = pluginCatalogue.GetPluginMeta(AuthenticationPluginId);
        if (plugin == null)
        {
            throw new NotFoundException(ResourceIds.AuthenticationPluginNotFoundException, AuthenticationPluginId);
        }

        var pluginConfig = (IAuthenticationPluginConfig)AuthenticationPluginConfigJObject.ToObject(plugin.PluginConfigType);

        AuthenticationPluginConfig = pluginConfig;
    }
}