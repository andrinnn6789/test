using System;

using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.ProcessEngineJob.Installation;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.ProcessEngineJob.Installation;

public class InstallJobTest
{
    private readonly Mock<IInstallationManager> _installationManagerMock;
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly Mock<IJobInfrastructure> _jobInfrastructureMock;
    private readonly InstallJob _installJob;

    public InstallJobTest()
    {
        _installationManagerMock = new Mock<IInstallationManager>();
        _serviceManagerMock = new Mock<IServiceManager>();
        _jobInfrastructureMock = new Mock<IJobInfrastructure>();

        _installJob = new InstallJob(_installationManagerMock.Object, _serviceManagerMock.Object);
    }

    [Fact]
    public void ExecuteJobTest()
    {
        string startedService = null;
        _serviceManagerMock.Setup(m => m.StartService(It.IsAny<string>()))
            .Callback<string>(service => startedService = service);

        _installJob.Parameter.ServiceToStart = "TestService";

        _installJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal("TestService", startedService);
        Assert.Null(_installJob.Result.InstanceName);

    }

    [Fact]
    public void ExecuteJobErrorTest()
    {
        _installationManagerMock.Setup(m => m.CreateOrUpdateInstallationAsync(It.IsAny<InstallationSetup>(), It.IsAny<IMessageLogger>()))
            .ThrowsAsync(new Exception());

        _installJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Failed, _installJob.Result.Result);
    }
}