using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.Extensions.Hosting;

namespace IAG.IdentityServer.Test.Configuration.Mock;

[PluginInfo("D2ACFD78-B223-41AA-9D7A-CE6E79074F29", "TestPluginName")]
public class TestPlugin : BaseAuthenticationPlugin<TestPluginConfig>
{
    public TestPlugin(IHostEnvironment environment): base(environment)
    {
        Config = new TestPluginConfig();
    }

    public static Guid Id => new("D2ACFD78-B223-41AA-9D7A-CE6E79074F29");

    public static string Name => "TestPluginName";

    [ExcludeFromCodeCoverage]
    public override IAuthenticationToken Authenticate(IRequestTokenParameter _)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public override string GetEMail(string user, Guid? tenantId)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public override void ChangePassword(string user, Guid? tenantId, string newPassword, bool changePasswordAfterLogin)
    {
        throw new NotImplementedException();
    }

    public override void AddClaimDefinitions(IEnumerable<ClaimDefinition> claimDefinitions)
    {
        throw new NotImplementedException();
    }
}