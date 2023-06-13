using System;
using System.Net.Http.Headers;
using System.Text;

namespace IAG.Infrastructure.Rest.Authentication;

public class BasicAuthentication : IAuthentication
{
    public string User { get; set; }

    public string Password { get; set; }

    public AuthenticationHeaderValue GetAuthorizationHeader()
    {
        var authenticationBytes = Encoding.ASCII.GetBytes($"{User}:{Password}");
        return new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(authenticationBytes));
    }
}