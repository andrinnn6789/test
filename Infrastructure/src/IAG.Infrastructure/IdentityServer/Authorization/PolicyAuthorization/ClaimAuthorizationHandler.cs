using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IAG.Infrastructure.Http;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimAuthorizationRequirement authorizationRequirement)
    {
        CheckRequirements(context, authorizationRequirement);

        return Task.CompletedTask;
    }

    public static void CheckRequirements(AuthorizationHandlerContext context,
        ClaimAuthorizationRequirement authorizationRequirement)
    {
        var userIsAnonymous = context.User.Identity == null || !context.User.Identities.Any(i => i.IsAuthenticated);
        if (userIsAnonymous)
            return;

        foreach (var requiredClaim in authorizationRequirement.RequiredClaims)
        {
            var claim = context.User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role &&
                ClaimHelper.MatchesClaimAndScope(c.Value, requiredClaim.ScopeName, requiredClaim.ClaimName));

            if (claim != null)
            {
                var permissions = ClaimHelper.GetPermissions(claim.Value);
                if ((permissions & requiredClaim.RequiredPermissions) == requiredClaim.RequiredPermissions)
                {
                    context.Succeed(authorizationRequirement);
                    return;
                }
            }
        }
    }
}

public class RemoteOnlyClaimAuthorizationHandler : AuthorizationHandler<ClaimAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimAuthorizationRequirement authorizationRequirement)
    {
        var httpContext = context.Resource as HttpContext
                          ?? (context.Resource as AuthorizationFilterContext)?.HttpContext;

        if (httpContext != null && HttpContextHelper.IsLocalRequest(httpContext))
        {
            context.Succeed(authorizationRequirement);
        }
        else
        {
            ClaimAuthorizationHandler.CheckRequirements(context, authorizationRequirement);
        }

        return Task.CompletedTask;
    }
}