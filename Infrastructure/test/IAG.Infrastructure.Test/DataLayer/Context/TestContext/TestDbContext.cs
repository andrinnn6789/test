using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.Test.DataLayer.Context.TestContext;

public class TestDbContext : BaseDbContext
{
    public static readonly string TestUserName = "testUserName";

    [UsedImplicitly]
    public DbSet<TestAEntity> TestAEntities { get; set; }
    [UsedImplicitly]
    public DbSet<TestBEntity> TestBEntities { get; set; }
    [UsedImplicitly]
    public DbSet<TestGenericEntity<string>> TestGenericEntities { get; set; }

    public TestDbContext(DbContextOptions options) : base(options, new ExplicitUserContext(TestUserName, null))
    {
    }
}