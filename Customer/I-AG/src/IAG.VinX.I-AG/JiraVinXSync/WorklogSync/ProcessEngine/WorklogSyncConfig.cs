using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.ProcessEngine;

public class WorklogSyncConfig : JobConfig<WorklogSyncJob>
{
    public WorklogSyncConfig()
    {
        JiraRestConfig = new HttpConfig {Authentication = new BasicAuthentication()};
    }

    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public HttpConfig JiraRestConfig { get; set; }
}