using System;

using IAG.Infrastructure.IdentityServer.Plugin;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Test.Configuration.Mock;

public class TestPluginConfig : IAuthenticationPluginConfig
{
    public TestPluginConfig()
    {
        Id = TestPlugin.Id;
        PluginName = TestPlugin.Name;
        ValidityDuration = new TimeSpan(0, 0, 42, 0);
        MySpecialSetting = "The answer is 42!";
    }

    [UsedImplicitly]
    public Guid Id { get; set; }

    [UsedImplicitly]
    public string PluginName { get; set; }

    [UsedImplicitly]
    public TimeSpan? ValidityDuration { get; set; }

    [UsedImplicitly]
    public bool Active { get; set; }

    [UsedImplicitly]
    public bool PublishClaims { get; set; }

    [UsedImplicitly]
    public string MySpecialSetting { get; set; }
}