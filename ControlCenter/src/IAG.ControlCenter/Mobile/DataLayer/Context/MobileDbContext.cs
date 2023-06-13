using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Mobile.DataLayer.Context;

[UsedImplicitly]
[ContextInfo("Mobile")]
public class MobileDbContext: ConfigCommonDbContext
{
    public MobileDbContext(DbContextOptions<MobileDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    //Mobile
    [UsedImplicitly]
    public DbSet<MobileInstallation> MobileInstallation { get; set; }

    [UsedImplicitly]
    public DbSet<MobileLicence> MobileLicences { get; set; }
        
    [UsedImplicitly]
    public DbSet<MobileModule> MobileModules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Mobile
        modelBuilder.Entity<MobileLicence>()
            .HasIndex(c => c.Licence).IsUnique();
        modelBuilder.Entity<MobileInstallation>()
            .HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<MobileModule>();
    }
}