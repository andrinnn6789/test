using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

public class NoAuthorizationHandler : AuthorizationHandler<ClaimAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ClaimAuthorizationRequirement authorizationRequirement)
    {
        context.Succeed(authorizationRequirement);

        return Task.CompletedTask;
    }
}