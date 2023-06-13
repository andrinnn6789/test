using System.Net.Http;

using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

namespace IAG.Infrastructure.IdentityServer.Authentication;

public class TokenRequest
{
    public IHttpConfig GetBearerConfig(string authEndpoint, RequestTokenParameter tokenRequest, string baseUrl, 
        IRequestResponseLogger logger)
    {
        var client = new RestClient(
            new HttpConfig
            {
                BaseUrl = authEndpoint
            }, 
            logger
        );
        var request = new JsonRestRequest(HttpMethod.Post, string.Empty);
        request.SetJsonBody(tokenRequest);
        var response = client.PostAsync<RequestTokenResponse>(request).Result;
        return new HttpConfig
        {
            BaseUrl = baseUrl,
            Authentication = new BearerAuthentication(response.AccessToken.Trim('"'))
        };
    }
}