using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

public class CommonConfig<T> : JobConfig<T>, ICommonConfig 
    where T: IJob
{
    public AtlasCredentials AtlasCredentials { get; set; }

    public HttpConfig LgavConfig { get; set; }

    public string LgavApiKey { get; set; }

    public string AtlasBasePath { get; set; }
}