using System.Collections.Generic;

using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;
using IAG.VinX.Smith.HelloTess.ProcessEngine;
using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;

using Moq;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.HelloTess.ProcessEngine;

public class HelloTessArticleSyncJobTest
{
    [Fact]
    public void ExecuteJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var job = new HelloTessMainSyncJob(factory, new MockILogger<HelloTessArticleCommonSyncJob>())
        {
            Config = new HelloTessMainSyncConfig
            {
                VinXConnectionString = factory.ConnectionString,
                HelloTessSystemConfigs = new List<HelloTessSystemConfig>()
            }
        };

        job.Config.HelloTessSystemConfigs.Add(ConfigHelper.SmithAndSmithSystemConfig);

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}