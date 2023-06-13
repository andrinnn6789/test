using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.IdentityServer.Configuration.DataLayer.Settings;

[UsedImplicitly]
[ContextInfo("IdentityServerSettings")]
public class IdentityDataStoreDbContext : ConfigCommonDbContext
{
    public IdentityDataStoreDbContext(DbContextOptions<IdentityDataStoreDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<ConfigDb> ConfigEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ConfigDb>().ToTable("ConfigIdentity");
    }
}