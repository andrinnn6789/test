using System;
using System.Collections.Generic;

using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

public abstract class ClaimProvider : IClaimProvider
{
    private readonly List<ClaimDefinition> _claimDefinitions;

    public IEnumerable<ClaimDefinition> ClaimDefinitions => _claimDefinitions;

    protected ClaimProvider()
    {
        _claimDefinitions = new List<ClaimDefinition>();
    }

    protected void AddClaimDefinition(Guid? tenantId, string scopeName, string claimName, PermissionKind possiblePermissions)
    {
        _claimDefinitions.Add(new ClaimDefinition()
        {
            TenantId = tenantId,
            ScopeName = scopeName,
            ClaimName = claimName,
            AvailablePermissions = possiblePermissions
        });
    }
}