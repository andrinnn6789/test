using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration.DataLayer.State;
using IAG.IdentityServer.Configuration.Model.Config;
using IAG.IdentityServer.Security;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Security;

public class AttackDetectionTest
{
    private readonly IdentityStateDbContext _context;
    private readonly Mock<IAttackDetectionConfig> _configuration;
        
    public AttackDetectionTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityStateDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new IdentityStateDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));

        _configuration = new Mock<IAttackDetectionConfig>();
        _configuration.SetupGet(m => m.ObservationPeriod).Returns(new TimeSpan(0, 0, 0, 1));
        _configuration.SetupGet(m => m.MaxFailedRequests).Returns(2);
    }

    [Fact]
    public async void RetryAfterBlockTest()
    {
        var attackDetection = new AttackDetection(_context, _configuration.Object);

        var ok1 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        var ok2 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        var block1 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        var block2 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");

        await Task.Delay(1100);
        var afterBlockOk = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");

        Assert.True(ok1);
        Assert.True(ok2);
        Assert.False(block1);
        Assert.False(block2);
        Assert.True(afterBlockOk);
    }

    [Fact]
    public async void ClearTest()
    {
        var attackDetection = new AttackDetection(_context, _configuration.Object);

        var ok1 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        var ok2 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        var block1 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        var block2 = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");
        await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");

        await attackDetection.ClearFailedRequests("testRealm", "testUser", "testRequest");
        var afterBlockOk = await attackDetection.CheckRequest("testRealm", "testUser", "testRequest");

        Assert.True(ok1);
        Assert.True(ok2);
        Assert.False(block1);
        Assert.False(block2);
        Assert.True(afterBlockOk);
    }

    [Fact]
    public async void SlowRetryTest()
    {
        var attackDetection = new AttackDetection(_context, _configuration.Object);

        var results = new List<bool>();
        for (int i = 0; i < 10; i++)
        {
            results.Add(await attackDetection.CheckRequest("testRealm", "testUser", "testRequest"));
            await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
            await Task.Delay(1000);
        }

        Assert.All(results, Assert.True);
    }

    [Fact]
    public async void DifferentRequestTest()
    {
        var attackDetection = new AttackDetection(_context, _configuration.Object);

        var results = new List<bool>();
        for (int i = 0; i < 1000; i++)
        {
            results.Add(await attackDetection.CheckRequest($"testRealm{i / 100}", $"testUser{i % 100}", "testRequest"));
            await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        }

        Assert.All(results, Assert.True);
    }

    [Fact]
    public void DefaultConfigTest()
    {
        var attackDetection = new AttackDetection(_context, new Mock<IAttackDetectionConfig>().Object);

        Assert.NotNull(attackDetection);
    }

    [Fact]
    public async void DatabaseExceptionTest()
    {
        _context.FailedRequestEntries = null;
        var attackDetection = new AttackDetection(_context, _configuration.Object);

        var results = new List<bool>();
        for (int i = 0; i < 20; i++)
        {
            results.Add(await attackDetection.CheckRequest("testRealm", "testUser", "testRequest"));
            await attackDetection.AddFailedRequest("testRealm", "testUser", "testRequest");
        }

        await attackDetection.ClearFailedRequests("testRealm", "testUser", "testRequest");

        Assert.All(results, Assert.True);
    }
}