using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authentication;

namespace IAG.IdentityServer.Authentication;

public interface IRefreshTokenManager
{
    Task<string> CreateRefreshTokenAsync(IAuthenticationToken authenticationToken);
    Task<IAuthenticationToken> CheckRefreshTokenAsync(string refreshToken);
}