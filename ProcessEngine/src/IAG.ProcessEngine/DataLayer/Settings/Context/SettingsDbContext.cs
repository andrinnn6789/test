using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.ProcessEngine.DataLayer.Settings.Model;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.ProcessEngine.DataLayer.Settings.Context;

[ContextInfo("ProcessEngineSettings")]
public class SettingsDbContext : ConfigCommonDbContext
{
    public SettingsDbContext(DbContextOptions<SettingsDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<JobConfig> ConfigEntries { get; set; }

    [UsedImplicitly]
    public DbSet<FollowUpJob> FollowUpJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobConfig>().ToTable("ConfigProcessEngine");
        modelBuilder.Entity<FollowUpJob>().Property(e => e.Id).ValueGeneratedNever();
    }
}