using System.Threading.Tasks;

namespace IAG.Infrastructure.IdentityServer;

public interface IAttackDetection
{
    Task<bool> CheckRequest(string realm, string user, string request = null);

    Task AddFailedRequest(string realm, string user, string request);

    Task ClearFailedRequests(string realm, string user, string request = null);
}