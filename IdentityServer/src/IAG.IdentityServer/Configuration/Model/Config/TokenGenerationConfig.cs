using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Configuration.Model.Config;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class TokenGenerationConfig : ITokenGenerationConfig
{
    public static readonly string ConfigName = "IdentityServer.TokenGeneration";

    public TokenGenerationConfig()
    {
        ExpirationTime = TimeSpan.FromMinutes(30);
        TokenValidationClockSkew = TimeSpan.FromSeconds(30);
    }

    public TimeSpan ExpirationTime { get; set; }
    public TimeSpan TokenValidationClockSkew { get; set; }
}