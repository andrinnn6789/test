using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.ProcessEngine.DataLayer.State.Model;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.ProcessEngine.DataLayer.State.Context;

[UsedImplicitly]
[ContextInfo("ProcessEngineState")]
public class ProcessEngineStateDbContext : BaseDbContext
{
    public ProcessEngineStateDbContext(DbContextOptions<ProcessEngineStateDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<JobData> JobDataEntries { get; set; }

    [UsedImplicitly]
    public DbSet<JobState> JobStateEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobData>().ToTable("JobData");
        modelBuilder.Entity<JobState>().ToTable("JobState")
            .HasMany(j => j.ChildJobs)
            .WithOne(j => j.ParentJob)
            .HasForeignKey(j => j.ParentJobId);
    }
}