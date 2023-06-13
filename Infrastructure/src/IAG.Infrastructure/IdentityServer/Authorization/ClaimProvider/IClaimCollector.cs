using System.Threading.Tasks;

namespace IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

public interface IClaimCollector
{
    Task CollectAndUpdateAsync();
}