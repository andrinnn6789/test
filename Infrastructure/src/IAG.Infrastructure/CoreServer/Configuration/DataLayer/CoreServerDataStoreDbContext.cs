using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.CoreServer.Configuration.DataLayer;

[UsedImplicitly]
[ContextInfo("CoreServer")]
public class CoreServerDataStoreDbContext : ConfigCommonDbContext
{
    public CoreServerDataStoreDbContext(DbContextOptions<CoreServerDataStoreDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<ConfigDb> ConfigEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ConfigDb>().ToTable("ConfigCoreServer");
    }
}