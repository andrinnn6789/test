using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.IntegrationTest.Authorization;

public class TestController : ControllerBase
{
    [HttpGet("test/AnonymousTest")]
    [AllowAnonymous]
    public string GetAnonymousTest()
    {
        return "Anonymous";
    }

    [HttpGet("test/AuthorizedTest")]
    [ClaimAuthorization("TestScope", "TestClaim", PermissionKind.Read)]
    public string GetAuthorizedTest()
    {
        return "Authorized";
    }
}