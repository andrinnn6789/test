using System;
using System.Collections.Generic;

using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.IdentityServer.Plugin;

public interface IAuthenticationPlugin
{
    Guid PluginId { get; }

    string PluginName { get; }

    string DefaultRealmName { get; }

    IAuthenticationPluginConfig Config { get; set; }

    void Init(IServiceProvider serviceProvider);

    IAuthenticationToken Authenticate(IRequestTokenParameter requestTokenParameter);

    string GetEMail(string user, Guid? tenant);

    void ChangePassword(string user, Guid? tenant, string newPassword, bool changePasswordAfterLogin);

    void AddClaimDefinitions(IEnumerable<ClaimDefinition> claimDefinitions);

    JObject GetExportData(IUserContext userContext);

    void ImportData(JObject data, IUserContext userContext);
}