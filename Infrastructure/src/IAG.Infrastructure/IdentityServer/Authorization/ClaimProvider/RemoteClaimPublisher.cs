using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.Rest;

using Microsoft.Extensions.Configuration;

namespace IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

public class RemoteClaimPublisher : IClaimPublisher
{
    private static readonly string TokenPath = $"{InfrastructureEndpoints.Auth}Realm/RequestToken";
    private static readonly string PublishPath = $"{InfrastructureEndpoints.Auth}Admin/RealmAdmin/PublishClaims";

    private readonly IConfiguration _configuration;

    public RemoteClaimPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishClaimsAsync(IEnumerable<ClaimDefinition> claimDefinitions)
    {
        var identityServerBaseUrl = _configuration["Authentication:IdentityServer"];
        if (string.IsNullOrEmpty(identityServerBaseUrl))
        {
            throw new System.Exception("Failed to publish claims to IdentityServer: No URL given");
        }

        var user = _configuration["Authentication:SystemUser"];
        var password = _configuration["Authentication:SystemPassword"];
        var requestTokenParam = RequestTokenParameter.ForPasswordGrant(user, password).ForRealm("Integrated");
        var config = new TokenRequest().GetBearerConfig(
            new Uri(new Uri(identityServerBaseUrl), TokenPath).ToString(),
            requestTokenParam, 
            identityServerBaseUrl, 
            null);

        var httpClient = new RestClient(config);
        var publishClaimsRequest = new JsonRestRequest(HttpMethod.Post, PublishPath);
        publishClaimsRequest.SetJsonBody(claimDefinitions);

        var response = await httpClient.ExecuteAsync(publishClaimsRequest);
        await response.CheckResponse();
    }
}