using System;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.Rest;
using IAG.InstallClient.Resource;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controller;
using IAG.Infrastructure.IdentityServer;

namespace IAG.InstallClient.BusinessLogic;

public class LoginManager : ILoginManager
{
    private readonly RestClient _httpClient;
    private readonly IRealmHandler _realmHandler;
    private readonly IAttackDetection _attackDetection;
    private readonly ITokenHandler _tokenHandler;

    public LoginManager(
        IConfiguration configuration, 
        IRealmHandler realmHandler, 
        IAttackDetection attackDetection, 
        ITokenHandler tokenHandler)
    {
        _realmHandler = realmHandler;
        _attackDetection = attackDetection;
        _tokenHandler = tokenHandler;
        var identityServerUrl = configuration["Authentication:IdentityServer"];
        if (!string.IsNullOrEmpty(identityServerUrl))
        {
            _httpClient = new RestClient(new HttpConfig()
            {
                BaseUrl = identityServerUrl
            });
        }
        else if (_realmHandler == null)
        {
            throw new LocalizableException(ResourceIds.ConfigIdentityServerUrlMissingError);
        }
    }

    public async Task<string> DoLoginAsync(string username, string password)
    {
        var requestTokenParameter = RequestTokenParameter.ForPasswordGrant(username, password)
            .ForRealm("Integrated");

        RequestTokenResponse response;
        if (_httpClient != null)
        {
            var request = new JsonRestRequest(HttpMethod.Post, InfrastructureEndpoints.AuthToken);
            request.SetJsonBody(requestTokenParameter);

            try
            {
                response = await _httpClient.PostAsync<RequestTokenResponse>(request);
            }
            catch (Exception)
            {
                throw new LocalizableException(ResourceIds.LoginFailedError);
            }
        }
        else
        {
            var actionResult = await _realmHandler.RequestToken(requestTokenParameter, _attackDetection, _tokenHandler);
            response = actionResult.Value;
            if (response == null)
            {
                throw new LocalizableException(ResourceIds.LoginFailedError);
            }
        }

        return response.AccessToken;
    }
}