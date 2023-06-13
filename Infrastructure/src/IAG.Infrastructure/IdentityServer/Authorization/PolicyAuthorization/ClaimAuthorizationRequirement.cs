using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Microsoft.AspNetCore.Authorization;

namespace IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationRequirement : IAuthorizationRequirement
{
    public List<RequiredClaim> RequiredClaims { get; }

    public ClaimAuthorizationRequirement(List<RequiredClaim> requiredClaims)
    {
        RequiredClaims = requiredClaims;
    }

    public override string ToString()
    {
        return string.Join(
            Environment.NewLine,
            RequiredClaims.Select(rc => ClaimHelper.ToString(rc.ScopeName, rc.ClaimName, rc.RequiredPermissions)
            ));
    }

    public static ClaimAuthorizationRequirement FromString(string claimString)
    {
        var requiredClaims = claimString.Split(Environment.NewLine)
            .Select(x =>
            {
                ClaimHelper.FromString(x, out string scopeName, out string claimName, out PermissionKind permissions);
                return new RequiredClaim(scopeName, claimName, permissions);
            })
            .ToList();

        return new ClaimAuthorizationRequirement(requiredClaims);
    }
}