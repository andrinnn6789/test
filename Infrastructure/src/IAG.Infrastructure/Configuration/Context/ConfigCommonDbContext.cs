using IAG.Infrastructure.Configuration.Model;
using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.Configuration.Context;

[UsedImplicitly]
[ContextInfo("ConfigCommon")]
public class ConfigCommonDbContext : BaseDbContext
{
    public ConfigCommonDbContext(DbContextOptions options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<ConfigCommon> ConfigCommonEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConfigCommon>().ToTable("ConfigCommon");
    }
}