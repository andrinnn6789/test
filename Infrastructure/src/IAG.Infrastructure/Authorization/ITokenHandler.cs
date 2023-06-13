using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.IdentityModel.Tokens;

namespace IAG.Infrastructure.Authorization;

public interface ITokenHandler
{
    bool? RequireHttpsMetadata { get; }

    string GetJwtToken(IAuthenticationToken authentication, string issuer);

    void CheckJwtToken(string token, string issuer);

    TokenValidationParameters GetTokenValidationParameters(string issuer);
}