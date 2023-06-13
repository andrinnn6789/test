using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.IdentityServer.Plugin;

[ExcludeFromCodeCoverage]
public class AuthenticationPluginConfig : IAuthenticationPluginConfig
{
    public string Id { get; set; }

    public bool Active { get; set; }

    public string PluginName { get; set; }

    public TimeSpan? ValidityDuration { get; set; }

    public bool PublishClaims { get; set; } = true;
}