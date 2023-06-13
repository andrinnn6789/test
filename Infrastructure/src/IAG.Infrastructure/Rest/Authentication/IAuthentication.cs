using System.Net.Http.Headers;

namespace IAG.Infrastructure.Rest.Authentication;

public interface IAuthentication
{
    AuthenticationHeaderValue GetAuthorizationHeader();
}