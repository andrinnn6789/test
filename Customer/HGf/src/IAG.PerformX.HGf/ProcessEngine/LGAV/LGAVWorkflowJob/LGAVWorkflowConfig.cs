using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

public class LgavWorkflowConfig : CommonConfig<LgavWorkflowJob>
{
    public LgavWorkflowConfig()
    {
        AtlasCredentials = new AtlasCredentials
        {
            BaseUrl = "http://localhost:$$Rest2Port$/rest2/",
        };
        LgavConfig = new HttpConfig
        {
            BaseUrl = "$$LgavUrl$"
        };
        LgavApiKey = "$$LgavApiKey$";
        AtlasBasePath = "$$LgavDocumentPath$";
    }
}