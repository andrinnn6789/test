using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;

using Microsoft.Extensions.Hosting;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.IdentityServer.Plugin;

[ExcludeFromCodeCoverage]
public abstract class BaseAuthenticationPlugin<TConfig> : IAuthenticationPlugin
    where TConfig : class, IAuthenticationPluginConfig, new()
{
    protected readonly IHostEnvironment Env;

    protected BaseAuthenticationPlugin(IHostEnvironment environment)
    {
        Env = environment;
        Config = new TConfig();

        var jobInfoAttribute = PluginInfoAttribute.GetPluginInfo(GetType());
        PluginId = jobInfoAttribute.PluginId;
        PluginName = jobInfoAttribute.PluginName;
    }

    public Guid PluginId { get; }

    public string PluginName { get; }

    public virtual string DefaultRealmName => null;

    public TConfig Config { get; set; }

    public virtual JObject Data
    {
        get => null;
        set => throw new NotImplementedException();
    }

    public virtual void Init(IServiceProvider serviceProvider)
    {
    }

    IAuthenticationPluginConfig IAuthenticationPlugin.Config
    {
        get => Config;
        set => Config = (TConfig)value;
    }

    public abstract IAuthenticationToken Authenticate(IRequestTokenParameter requestTokenParameter);

    public abstract string GetEMail(string user, Guid? tenant);

    public abstract void ChangePassword(string user, Guid? tenant, string newPassword, bool changePasswordAfterLogin);

    public abstract void AddClaimDefinitions(IEnumerable<ClaimDefinition> claimDefinitions);
        
    public virtual JObject GetExportData(IUserContext userContext)
    {
        return default;
    }

    public virtual void ImportData(JObject data, IUserContext userContext)
    {
        throw new NotImplementedException();
    }
}