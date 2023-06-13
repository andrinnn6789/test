using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Context.TestContext;

public class TestDbContext : BaseDbContext
{
    [UsedImplicitly]
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestDbContext(DbContextOptions options) : base(options, new ExplicitUserContext("test", null))
    {
    }
}