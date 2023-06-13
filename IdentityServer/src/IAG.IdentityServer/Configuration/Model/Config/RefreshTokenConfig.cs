using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Configuration.Model.Config;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class RefreshTokenConfig : IRefreshTokenConfig
{
    public static readonly string ConfigName = "IdentityServer.RefreshToken";

    public RefreshTokenConfig()
    {
        ExpirationTime = TimeSpan.FromDays(1);
    }

    public TimeSpan ExpirationTime { get; set;  } 
}