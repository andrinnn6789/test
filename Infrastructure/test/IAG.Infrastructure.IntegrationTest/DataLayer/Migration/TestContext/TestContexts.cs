using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext;

[ContextInfo("TestProject")]
public class TestProjectDbContext : BaseDbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestProjectDbContext(DbContextOptions options) : base(options, new ExplicitUserContext("test", null))
    {
    }
}