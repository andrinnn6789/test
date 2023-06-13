using System.IO;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncLinkList;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.ProcessEngine.SyncLinkList;

public class SyncLinkListJobTest
{
    [Fact]
    public async Task ExecuteSyncLinkListJobTestWithKestrelMock()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".RequestMock.json", testPort);

        bool result;
        SyncResult jobResult;
        var testPath = Path.Combine(Path.GetTempPath(), "SyncLinkListJobTest");
        var linkListContent = @"{""Links"":[
                { ""Name"": ""Notepad++"", ""Link"": ""https://notepad-plus-plus.org/downloads/""}
            ]}";
        try
        {
            var linkListsPath = Path.Combine(testPath, "LinkLists");
            var testLinkList = Path.Combine(linkListsPath, "linkList.json");
            Directory.CreateDirectory(linkListsPath);
            await File.WriteAllTextAsync(testLinkList, linkListContent, Encoding.UTF8);

            var job = BuildJob(linkListsPath, new BackendConfig { UrlControlCenter = $"http://localhost:{testPort}/" }, ConfigHelper.GetMockedTokenRequest());
            var jobInfrastructureMock = new Mock<IJobInfrastructure>();
            result = job.Execute(jobInfrastructureMock.Object);
            jobResult = job.Result;
        }
        finally
        {
            Directory.Delete(testPath, true);
        }

        Assert.True(result);
        Assert.Equal(1, jobResult.SuccessCount);
    }

    [Fact(Skip = "Only executed manually")]
    public void ExecuteSyncLinkListJobWithLocalCc()
    {
        var job = BuildJob("L:\\I-AG Software\\BPE\\Link-Listen", new BackendConfig { UrlControlCenter = "http://localhost:8086/" }, ConfigHelper.GetMockedTokenRequest());
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }

    [Fact(Skip = "Only executed manually")]
    public void ExecuteSyncLinkListJobWithTestingCc()
    {
        var job = BuildJob("L:\\I-AG Software\\BPE\\Link-Listen", ConfigHelper.CcConfigTestingServer, new ControlCenterTokenRequest());
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }

    private SyncLinkListJob BuildJob(string linkListsPath, BackendConfig backendConfig, IControlCenterTokenRequest tokenRequestHandler)
    {
        return new(tokenRequestHandler, new MockILogger<SyncLinkListJob>())
        {
            Config =
            {
                Backend = backendConfig,
                LinkListsPath = linkListsPath,
            }
        };
    }
}