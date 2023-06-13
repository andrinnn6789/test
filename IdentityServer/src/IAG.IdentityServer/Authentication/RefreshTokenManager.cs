using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration.DataLayer.State;
using IAG.IdentityServer.Configuration.Model.Config;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.IdentityServer.Authentication;

public class RefreshTokenManager : IRefreshTokenManager
{
    private readonly IdentityStateDbContext _context;
    private readonly TimeSpan _expirationTime;

    public RefreshTokenManager(IdentityStateDbContext context, IRefreshTokenConfig config)
    {
        _context = context;
        _expirationTime = config.ExpirationTime;
    }

    public async Task<string> CreateRefreshTokenAsync(IAuthenticationToken authenticationToken)
    {
        try
        {
            ClearRefreshTokensOfUser(authenticationToken.Username, authenticationToken.Tenant);
            return await CreateRefreshTokenAsync(authenticationToken.Username, authenticationToken.Tenant, JsonSerializer.Serialize(authenticationToken), null);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<IAuthenticationToken> CheckRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        try
        {
            var newerToken = await _context.RefreshTokenEntries.FirstOrDefaultAsync(x => x.PreviousRefreshToken == refreshToken);
            if (newerToken != null)
            {
                // Token reused. May an attack! Invalidate all refresh tokens for that user.
                ClearRefreshTokensOfUser(newerToken.User, newerToken.Tenant);
                await _context.SaveChangesAsync();

                return null;
            }

            if (!Guid.TryParse(refreshToken, out var tokenId))
                return null;

            var currentToken = await _context.RefreshTokenEntries.FirstOrDefaultAsync(x => x.Id == tokenId);
            if (currentToken == null || currentToken.Timestamp + _expirationTime < DateTime.UtcNow) 
                return null;

            var newRefreshToken = await CreateRefreshTokenAsync(currentToken.User, currentToken.Tenant, currentToken.AuthenticationToken, refreshToken);

            var authenticationToken = JsonSerializer.Deserialize<AuthenticationToken>(currentToken.AuthenticationToken, new JsonSerializerOptions()
            {
                Converters = { new ClaimJsonConverter() }
            });
            // ReSharper disable once PossibleNullReferenceException - I trust my input. Otherwise a NullReferenceException would have the same effect and return null
            authenticationToken.RefreshToken = newRefreshToken;

            return authenticationToken;
        }
        catch (Exception)
        {
            return null;
        }
    }


    private async Task<string> CreateRefreshTokenAsync(string user, Guid? tenant, string authenticationToken, string currentToken)
    {
        var entry = new RefreshTokenDb()
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            PreviousRefreshToken = currentToken,
            User = user,
            Tenant = tenant,
            AuthenticationToken = authenticationToken
        };

        _context.RefreshTokenEntries.Add(entry);
        await _context.SaveChangesAsync();

        return entry.Id.ToString();
    }

    private void ClearRefreshTokensOfUser(string user, Guid? tenant)
    {
        var allTokensOfUser = _context.RefreshTokenEntries.Where(x => x.User == user && x.Tenant == tenant);
        _context.RemoveRange(allTokensOfUser);
    }
}