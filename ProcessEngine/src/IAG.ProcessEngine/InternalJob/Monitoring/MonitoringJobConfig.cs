using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.Settings;

namespace IAG.ProcessEngine.InternalJob.Monitoring;

public class MonitoringJobConfig : JobConfig<MonitoringJob>
{
    public MonitoringJobConfig()
    {
        CronExpression = "0/15 * * * *";  // every 15 minutes by default
        Active = true;
    }
    public string CustomerName { get; set; } = $"$${SettingsConst.CustomerName}$";
    public JobResultEnum MinExportState { get; set; } = JobResultEnum.PartialSuccess;
}