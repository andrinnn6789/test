using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.ProcessEngine;

public class IssueSyncConfig : JobConfig<IssueSyncJob>
{
    public IssueSyncConfig()
    {
        JiraRestConfig = new HttpConfig {Authentication = new BasicAuthentication()};
    }

    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public HttpConfig JiraRestConfig { get; set; }
}