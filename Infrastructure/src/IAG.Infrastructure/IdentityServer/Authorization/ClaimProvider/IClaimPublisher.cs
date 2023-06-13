using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

public interface IClaimPublisher
{
    Task PublishClaimsAsync(IEnumerable<ClaimDefinition> claimDefinitions);
}