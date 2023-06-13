using IAG.Infrastructure.ProcessEngine.Configuration;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.InternalJob.Cleanup;

[UsedImplicitly]
public class CleanupJobConfig : JobConfig<CleanupJob>
{
    public CleanupJobConfig()
    {
        CronExpression = "0 21 * * *";
    }

    [UsedImplicitly]
    public int ArchiveDays { get; set; } = 30;

    [UsedImplicitly]
    public int ErrorDays { get; set; } = 60;
}