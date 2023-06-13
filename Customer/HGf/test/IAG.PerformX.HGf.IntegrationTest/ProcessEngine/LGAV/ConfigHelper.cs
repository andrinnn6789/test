using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

namespace IAG.PerformX.HGf.IntegrationTest.ProcessEngine.LGAV;

public static class ConfigHelper<T> where T: IJob
{
    public static CommonConfig<T> CommonConfig =>
        new()
        {
            AtlasCredentials = new AtlasCredentials
            {
                BaseUrl = "http://localhost:8084/rest2/",
                User = "ProcessEngine", 
                Password = "d8Add3DD2n2dD4PI"
            },
            LgavConfig = new HttpConfig
            {
                BaseUrl = "https://test.l-gav.com/eduapi/"
            },
            LgavApiKey = "4SKPX8GuBNcwJ4L54QLF",
            AtlasBasePath = "K:\\HGf"
        };
}