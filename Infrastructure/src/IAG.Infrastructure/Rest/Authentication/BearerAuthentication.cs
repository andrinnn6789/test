using System.Net.Http.Headers;

namespace IAG.Infrastructure.Rest.Authentication;

public class BearerAuthentication : IAuthentication
{
    public string Token { get; }

    public BearerAuthentication(string token)
    {
        Token = token;
    }

    public AuthenticationHeaderValue GetAuthorizationHeader()
    {
        return new("Bearer", Token);
    }
}