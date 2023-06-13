using IAG.IdentityServer.Authentication;
using IAG.IdentityServer.Configuration.DataLayer.State;
using IAG.IdentityServer.Configuration.Model.Config;
using IAG.Infrastructure.IdentityServer.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IAG.IdentityServer.Test.Authentication;

public class RefreshTokenManagerTest
{
    private readonly IdentityStateDbContext _dbContext;
    private readonly RefreshTokenManager _refreshTokenManager;

    public RefreshTokenManagerTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityStateDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _dbContext = new IdentityStateDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));

        var config = new RefreshTokenConfig { ExpirationTime = TimeSpan.FromDays(1) };
        _refreshTokenManager = new RefreshTokenManager(_dbContext, config);
    }

    [Fact]
    public async Task CreateRefreshTokenSuccessTest()
    {
        var authenticationToken = new AuthenticationToken
        {
            Username = "TestUser"
        };
        _dbContext.RefreshTokenEntries.Add(new RefreshTokenDb
        {
            Id = Guid.NewGuid(),
            User = authenticationToken.Username
        });
        await _dbContext.SaveChangesAsync();

        string refreshToken = await _refreshTokenManager.CreateRefreshTokenAsync(authenticationToken);

        var refreshTokenEntry = Assert.Single(_dbContext.RefreshTokenEntries);
        Assert.NotNull(refreshTokenEntry);
        Assert.NotEmpty(refreshTokenEntry.AuthenticationToken);
        Assert.Null(refreshTokenEntry.PreviousRefreshToken);
        Assert.Equal(authenticationToken.Username, refreshTokenEntry.User);
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
    }

    [Fact]
    public async Task CreateRefreshTokenExceptionTest()
    {
        _dbContext.RefreshTokenEntries = null;

        string refreshToken = await _refreshTokenManager.CreateRefreshTokenAsync(new AuthenticationToken());

        Assert.Null(refreshToken);
    }

    [Fact]
    public async Task CheckRefreshTokenFlowTest()
    {
        var testTenant = Guid.NewGuid();
        var authenticationTokenA = new AuthenticationToken
        {
            Username = "TestUserA",
            Claims = new List<Claim> { new Claim("TestClaim", "TestValue") }
        };
        var authenticationTokenB = new AuthenticationToken
        {
            Username = "TestUserB"
        };
        var authenticationTokenAWithTenant = new AuthenticationToken
        {
            Username = "TestUserA",
            Tenant = testTenant
        };

        var initialRefreshTokenA = await _refreshTokenManager.CreateRefreshTokenAsync(authenticationTokenA);
        var firstAuthTokenA = await _refreshTokenManager.CheckRefreshTokenAsync(initialRefreshTokenA);
        var initialRefreshTokenB = await _refreshTokenManager.CreateRefreshTokenAsync(authenticationTokenB);
        var firstAuthTokenB = await _refreshTokenManager.CheckRefreshTokenAsync(initialRefreshTokenB);
        var secondAuthTokenB = await _refreshTokenManager.CheckRefreshTokenAsync(firstAuthTokenB?.RefreshToken);
        var initialRefreshTokenAWithTenant = await _refreshTokenManager.CreateRefreshTokenAsync(authenticationTokenAWithTenant);
        var secondAuthTokenA = await _refreshTokenManager.CheckRefreshTokenAsync(firstAuthTokenA?.RefreshToken);

        var firstTokenRetryA = await _refreshTokenManager.CheckRefreshTokenAsync(firstAuthTokenA?.RefreshToken);
        var secondTokenRetryA = await _refreshTokenManager.CheckRefreshTokenAsync(secondAuthTokenA?.RefreshToken);
        var firstTokenRetryB = await _refreshTokenManager.CheckRefreshTokenAsync(firstAuthTokenB?.RefreshToken);
        var secondTokenRetryB = await _refreshTokenManager.CheckRefreshTokenAsync(secondAuthTokenB?.RefreshToken);

        var firstAuthTokenAWithTenant = await _refreshTokenManager.CheckRefreshTokenAsync(initialRefreshTokenAWithTenant);
        var secondAuthTokenAWithTenant = await _refreshTokenManager.CheckRefreshTokenAsync(firstAuthTokenAWithTenant?.RefreshToken);
        var firstTokenRetryAWithTenant = await _refreshTokenManager.CheckRefreshTokenAsync(firstAuthTokenAWithTenant?.RefreshToken);
        var secondTokenRetryAWithTenant = await _refreshTokenManager.CheckRefreshTokenAsync(secondAuthTokenAWithTenant?.RefreshToken);

        Assert.NotNull(initialRefreshTokenA);
        Assert.NotNull(firstAuthTokenA);
        Assert.NotNull(firstAuthTokenA.RefreshToken);
        Assert.NotNull(firstAuthTokenA.Claims);
        Assert.Equal("TestValue", Assert.Single(firstAuthTokenA.Claims).Value);
        Assert.NotNull(secondAuthTokenA);
        Assert.NotNull(secondAuthTokenA.RefreshToken);
        Assert.NotNull(secondAuthTokenA.Claims);
        Assert.Equal("TestValue", Assert.Single(secondAuthTokenA.Claims).Value);
        Assert.Null(firstTokenRetryA);
        Assert.Null(secondTokenRetryA);

        Assert.NotNull(initialRefreshTokenB);
        Assert.NotNull(firstAuthTokenB);
        Assert.NotNull(firstAuthTokenB.RefreshToken);
        Assert.NotNull(secondAuthTokenB);
        Assert.NotNull(secondAuthTokenB.RefreshToken);
        Assert.Null(firstTokenRetryB);
        Assert.Null(secondTokenRetryB);

        Assert.NotNull(initialRefreshTokenAWithTenant);
        Assert.NotNull(firstAuthTokenAWithTenant);
        Assert.NotNull(firstAuthTokenAWithTenant.RefreshToken);
        Assert.Equal(testTenant, firstAuthTokenAWithTenant.Tenant);
        Assert.NotNull(secondAuthTokenAWithTenant);
        Assert.NotNull(secondAuthTokenAWithTenant.RefreshToken);
        Assert.Equal(testTenant, secondAuthTokenAWithTenant.Tenant);
        Assert.Null(firstTokenRetryAWithTenant);
        Assert.Null(secondTokenRetryAWithTenant);
    }


    [Fact]
    public async Task CheckRefreshTokenExceptionTest()
    {
        var expiredTokenId = Guid.NewGuid();
        var corruptedTokenId = Guid.NewGuid();
        _dbContext.RefreshTokenEntries.Add(new RefreshTokenDb
        {
            Id = expiredTokenId,
            User = "TestUser",
            Timestamp = DateTime.UtcNow.AddDays(-2)
        });
        _dbContext.RefreshTokenEntries.Add(new RefreshTokenDb
        {
            Id = corruptedTokenId,
            User = "TestUser",
            Timestamp = DateTime.UtcNow,
            AuthenticationToken = null
        });
        await _dbContext.SaveChangesAsync();

        var nullResult = await _refreshTokenManager.CheckRefreshTokenAsync(null);
        var emptyResult = await _refreshTokenManager.CheckRefreshTokenAsync(string.Empty);
        var invalidTokenResult = await _refreshTokenManager.CheckRefreshTokenAsync("InvalidToken");
        var expiredResult = await _refreshTokenManager.CheckRefreshTokenAsync(expiredTokenId.ToString());
        var corruptedResult = await _refreshTokenManager.CheckRefreshTokenAsync(corruptedTokenId.ToString());

        Assert.Null(nullResult);
        Assert.Null(emptyResult);
        Assert.Null(invalidTokenResult);
        Assert.Null(expiredResult);
        Assert.Null(corruptedResult);
    }

}