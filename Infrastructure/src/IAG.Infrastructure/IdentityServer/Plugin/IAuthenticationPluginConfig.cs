using System;

namespace IAG.Infrastructure.IdentityServer.Plugin;

public interface IAuthenticationPluginConfig
{
    /// <summary>
    /// Timespan after which the authentication token should valid (before it will expire)
    /// </summary>
    TimeSpan? ValidityDuration { get; }
        
    bool Active { get; set; }

    bool PublishClaims { get; }
}