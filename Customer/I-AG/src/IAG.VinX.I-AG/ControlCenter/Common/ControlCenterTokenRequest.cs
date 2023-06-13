using System;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.ControlCenter.Config;

namespace IAG.VinX.IAG.ControlCenter.Common;

public class ControlCenterTokenRequest: TokenRequest, IControlCenterTokenRequest
{
    public IHttpConfig GetConfig(BackendConfig backendConfig, string endpoint, IRequestResponseLogger logger)
    {
        var tokenRequestBody = RequestTokenParameter.ForPasswordGrant(backendConfig.Username, backendConfig.Password)
            .ForRealm(backendConfig.Realm);

        return GetBearerConfig(
            new Uri(new Uri(backendConfig.UrlAuth), InfrastructureEndpoints.AuthToken).ToString(),
            tokenRequestBody, new Uri(new Uri(backendConfig.UrlControlCenter), endpoint).ToString(),
            logger);
    }
}