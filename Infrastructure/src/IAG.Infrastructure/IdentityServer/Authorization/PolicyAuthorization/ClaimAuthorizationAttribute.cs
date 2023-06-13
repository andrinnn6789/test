using System;
using System.Collections.Generic;

using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Microsoft.AspNetCore.Authorization;

namespace IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationAttribute : AuthorizeAttribute
{
    public static readonly string PolicyPrefix = "IagClaim_";

    private ClaimAuthorizationAttribute(List<RequiredClaim> requiredClaims)
    {
        Requirement = new ClaimAuthorizationRequirement(requiredClaims);
    }

    public ClaimAuthorizationAttribute(string scopeName, string claimName, PermissionKind permissions)
        : this(new List<RequiredClaim>() 
        {
            new(scopeName, claimName, permissions)
        })
    { 
    }

    public ClaimAuthorizationAttribute(
        string scopeName1, string claimName1, PermissionKind permissions1,
        string scopeName2, string claimName2, PermissionKind permissions2
    ) : this(new List<RequiredClaim>()
    {
        new(scopeName1, claimName1, permissions1),
        new(scopeName2, claimName2, permissions2)
    })
    {
    }

    public ClaimAuthorizationAttribute(
        string scopeName1, string claimName1, PermissionKind permissions1,
        string scopeName2, string claimName2, PermissionKind permissions2,
        string scopeName3, string claimName3, PermissionKind permissions3
    ) : this(new List<RequiredClaim>()
    {
        new(scopeName1, claimName1, permissions1),
        new(scopeName2, claimName2, permissions2),
        new(scopeName3, claimName3, permissions3)
    })
    {
    }

    public ClaimAuthorizationAttribute(
        string scopeName1, string claimName1, PermissionKind permissions1,
        string scopeName2, string claimName2, PermissionKind permissions2,
        string scopeName3, string claimName3, PermissionKind permissions3,
        string scopeName4, string claimName4, PermissionKind permissions4
    ) : this(new List<RequiredClaim>()
    {
        new(scopeName1, claimName1, permissions1),
        new(scopeName2, claimName2, permissions2),
        new(scopeName3, claimName3, permissions3),
        new(scopeName4, claimName4, permissions4)
    })
    {
    }

    public ClaimAuthorizationRequirement Requirement
    {
        get
        {
            if (Policy?.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase) != true)
            {
                return null;
            }

            return ClaimAuthorizationRequirement.FromString(Policy.Substring(PolicyPrefix.Length));
        }
        set => Policy = $"{PolicyPrefix}{value}";
    }
}