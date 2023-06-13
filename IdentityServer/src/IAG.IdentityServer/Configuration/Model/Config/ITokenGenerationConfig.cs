using System;

namespace IAG.IdentityServer.Configuration.Model.Config;

public interface ITokenGenerationConfig
{
    TimeSpan ExpirationTime { get; }
        
    TimeSpan TokenValidationClockSkew { get; }
}