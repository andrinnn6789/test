using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

public interface ICommonConfig
{
    AtlasCredentials AtlasCredentials { get; set; }

    HttpConfig LgavConfig { get; set; }

    public string LgavApiKey { get; set; }

    string AtlasBasePath { get; set; }
}