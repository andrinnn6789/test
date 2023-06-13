using System.Collections.Generic;
using System.IO;
using System.Reflection;

using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.PublishReleases;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.ProcessEngine.PublishReleases;

public class PublishReleasesJobTest
{
    [Fact]
    public void ExecutePublishReleasesJobWithKestrelMock()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".RequestMock.json", testPort);

        bool result;
        SyncResult jobResult;
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var testPath = Path.Combine(Path.GetTempPath(), "PublishReleasesJobTest");
        try
        {
            var artifactsPath = Path.Combine(testPath, "Artifacts");
            var testCustomerProductPath = Path.Combine(artifactsPath, "Master", "Customer", "TestProduct", "publish");
            CopyAllFiles(assemblyPath, testCustomerProductPath);

            var settingsPath = Path.Combine(testPath, "Master", "Settings");
            Directory.CreateDirectory(settingsPath);

            var job = BuildJob(artifactsPath, settingsPath, new BackendConfig { UrlControlCenter = $"http://localhost:{testPort}/" }, ConfigHelper.GetMockedTokenRequest());
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
    public void ExecutePublishReleasesJobWithLocalCc()
    {
        var job = BuildJob("A:\\DotNet\\IAG", "L:\\I-AG Software\\BPE\\Settings-Templates", new BackendConfig { UrlControlCenter = "http://localhost:8086/" }, ConfigHelper.GetMockedTokenRequest());
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }

    [Fact(Skip = "Only executed manually")]
//        [Fact]
    public void ExecutePublishReleasesJobWithTestingCc()
    {
        var job = BuildJob("A:\\DotNet\\IAG", "L:\\I-AG Software\\BPE\\Settings-Templates", ConfigHelper.CcConfigTestingServer, new ControlCenterTokenRequest());
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }

    private PublishReleasesJob BuildJob(string artifactsPath, string settingsPath, BackendConfig backendConfig, IControlCenterTokenRequest tokenRequestHandler)
    {
        return new(tokenRequestHandler, new MockILogger<PublishReleasesJob>())
        {
            Config =
            {
                Backend = backendConfig,
                ArtifactsPath = artifactsPath,
                SettingsPath = settingsPath,
                ReleasePaths = new Dictionary<string, string>
                {
                    {ArtifactsScanner.PerformXDirectoryName, "BPE"},
                    {ArtifactsScanner.VinXDirectoryName, "BPE"},
                    {SettingsScanner.SettingsDirectoryName, "Settings"}
                }
            }
        };
    }

    private void CopyAllFiles(string sourceDirectory, string targetDirectory)
    {
        foreach (var sourceFile in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var targetFile = Path.Combine(targetDirectory, Path.GetRelativePath(sourceDirectory, sourceFile));
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile) ?? string.Empty);
            File.Copy(sourceFile, targetFile);
        }
    }
}