using System.Security.Claims;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace IAG.Infrastructure.IntegrationTest.Authorization;

public class TestJwtBearerPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string name, JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = OnMessageReceived,
        };

        new JwtBearerPostConfigureOptions().PostConfigure(name, options);
    }

    private Task OnMessageReceived(MessageReceivedContext arg)
    {
        if (!string.IsNullOrEmpty(arg.HttpContext.Request.Headers[HeaderNames.Authorization]))
        {
            var claimValue = ClaimHelper.ToString("TestScope", "TestClaim", PermissionKind.Read);
            var claims = new[] { new Claim(ClaimTypes.Role, claimValue) };
            arg.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestOnly"));
            arg.Success();
        }

        return Task.CompletedTask;
    }
}