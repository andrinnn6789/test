using System.Collections.Generic;

using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

public interface IClaimProvider
{
    IEnumerable<ClaimDefinition> ClaimDefinitions { get; }
}