using System;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.ProcessEngineJob.Transfer;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.ProcessEngineJob.Transfer;

public class TransferJobTest
{
    private readonly Mock<IInstallationManager> _installationManagerMock;
    private readonly Mock<IInventoryManager> _inventoryManagerMock;
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly Mock<IJobInfrastructure> _jobInfrastructureMock;
    private readonly TransferJob _transferJob;

    public TransferJobTest()
    {
        _installationManagerMock = new Mock<IInstallationManager>();
        _inventoryManagerMock = new Mock<IInventoryManager>();
        _serviceManagerMock = new Mock<IServiceManager>();
        _jobInfrastructureMock = new Mock<IJobInfrastructure>();

        _transferJob = new TransferJob(_installationManagerMock.Object, _inventoryManagerMock.Object, _serviceManagerMock.Object);
    }

    [Fact]
    public void ExecuteJobTest()
    {
        string sourceInstance = null;
        string targetInstance = null;
        _installationManagerMock.Setup(m => m.TransferInstance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageLogger>()))
            .Callback<string, string, IMessageLogger>((source, target, _) =>
            {
                sourceInstance = source;
                targetInstance = target;
            });

        string registeredInstance = null;
        _inventoryManagerMock.Setup(m => m.RegisterInstallationAsync(It.IsAny<Guid>(), It.IsAny<InstallationRegistration>()))
            .Callback<Guid, InstallationRegistration>((_, registration) =>
            {
                registeredInstance = registration.InstanceName;
            });

        string startedService = null;
        _serviceManagerMock.Setup(m => m.StartService(It.IsAny<string>()))
            .Callback<string>(service => startedService = service);

        _transferJob.Parameter.SourceInstanceName = "SourceInstance";
        _transferJob.Parameter.TargetInstanceName = "TargetInstance";
        _transferJob.Parameter.ServiceToStart = "TestService";

        _transferJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal("SourceInstance", sourceInstance);
        Assert.Equal("TargetInstance", registeredInstance);
        Assert.Equal("TargetInstance", targetInstance);
        Assert.Equal("TestService", startedService);
    }

    [Fact]
    public void ExecuteJobErrorTest()
    {
        _installationManagerMock.Setup(m => m.TransferInstance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageLogger>()))
            .Throws(new Exception());

        _transferJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Failed, _transferJob.Result.Result);
    }
}