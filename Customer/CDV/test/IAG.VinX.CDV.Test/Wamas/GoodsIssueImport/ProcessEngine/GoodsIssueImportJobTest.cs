using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.GoodsIssueImport.ProcessEngine;

public class GoodsIssueImportJobTest
{
    [Fact]
    public void ExecuteGoodsIssueImportJob()
    {
        var fakeConfig = new Mock<GoodsIssueImportJobConfig>
        {
            Object =
            {
                WamasFtpConfig = new WamasFtpConfig(),
                ConnectionString = "test"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new WamasImportJobResult
        {
            Result = JobResultEnum.Success,
            ImportedCount = 1,
            ErrorCount = 0
        };

        var fakeGoodsIssueImporter = new Mock<IGoodsIssueImporter>();
        fakeGoodsIssueImporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeGoodsIssueImporter.Setup(exp => exp.ImportGoodsIssues()).Returns(fakeJobResult);

        var job = new GoodsIssueImportJob(fakeGoodsIssueImporter.Object)
        {
            Config = fakeConfig.Object
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        job.Execute(jobInfrastructureMock.Object);
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ImportedCount == fakeJobResult.ImportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}