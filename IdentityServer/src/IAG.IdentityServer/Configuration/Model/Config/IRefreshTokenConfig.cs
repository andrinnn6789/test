using System;

namespace IAG.IdentityServer.Configuration.Model.Config;

public interface IRefreshTokenConfig
{
    TimeSpan ExpirationTime { get; }
}