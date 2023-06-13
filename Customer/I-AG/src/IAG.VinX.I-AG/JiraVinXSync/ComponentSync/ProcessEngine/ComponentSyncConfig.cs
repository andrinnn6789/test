using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.ProcessEngine;

public class ComponentSyncConfig : JobConfig<ComponentSyncJob>
{
    public ComponentSyncConfig()
    {
        JiraRestConfig = new HttpConfig {Authentication = new BasicAuthentication()};
    }

    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public HttpConfig JiraRestConfig { get; set; }
}