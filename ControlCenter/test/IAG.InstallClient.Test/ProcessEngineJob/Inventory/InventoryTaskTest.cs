using System;
using System.Collections.Generic;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Influx;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.ProcessEngineJob.Inventory;

using InfluxDB.Client.Writes;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.ProcessEngineJob.Inventory;

public class InventoryJobTest
{
    private readonly InventoryJob _inventoryJob;
    private CustomerInfo _customerInfo;
    private readonly Mock<IJobInfrastructure> _jobInfrastructureMock;

    private List<PointData> _sendDataPoints = new();
    private List<InstalledRelease> _installations;
    private readonly List<MessageStructure> _jobMsgLog = new();

    public InventoryJobTest()
    {
        var installationManagerMock = new Mock<IInstallationManager>();
        installationManagerMock.Setup(m => m.CurrentSelfVersion).Returns("SelfVersion");
        installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(() => _installations ?? throw new Exception());
            
        var customerManagerMock = new Mock<ICustomerManager>();
        customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(() => _customerInfo);
            
        var influxClientMock = new Mock<IInfluxClient>();
        influxClientMock.Setup(m => m.SendDataPointAsync(It.IsAny<string>(), It.IsAny<PointData>()))
            .Callback<string, PointData>((_, dp) => _sendDataPoints.Add(dp));

        _jobInfrastructureMock = new Mock<IJobInfrastructure>();
        _jobInfrastructureMock.Setup(m => m.AddMessage(It.IsAny<MessageStructure>()))
            .Callback<MessageStructure>(msg => _jobMsgLog.Add(msg));

        _inventoryJob = new InventoryJob(installationManagerMock.Object, customerManagerMock.Object, influxClientMock.Object);
        _customerInfo = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "MyCustomer",
        };
    }

    [Fact]
    public void ExecuteJobSuccessTest()
    {
        var installation = new InstalledRelease
        {
            ProductName = "MyProduct",
            InstanceName = "MyInstance",
            Version = "MyVersion",
            CustomerPluginName = "MyCustomerPlugin"
        };
        _installations = new List<InstalledRelease> {installation};

        _inventoryJob.Execute(_jobInfrastructureMock.Object);

        Assert.Single(_sendDataPoints);
        Assert.All(_sendDataPoints, dataPoint => Assert.True(dataPoint.HasFields()));
        Assert.All(_sendDataPoints, dataPoint => Assert.Contains(_customerInfo.CustomerName, dataPoint.ToLineProtocol()));
        Assert.All(_sendDataPoints, dataPoint => Assert.Contains(_customerInfo.Id.ToString(), dataPoint.ToLineProtocol()));
        Assert.Contains("SelfVersion", _sendDataPoints[0].ToLineProtocol());
    }

    [Fact]
    public void ExecuteJobWithoutCustomerTest()
    {
        _customerInfo = null;

        _inventoryJob.Execute(_jobInfrastructureMock.Object);

        Assert.Empty(_sendDataPoints);
        Assert.NotEmpty(_jobMsgLog);
    }

    [Fact]
    public void ExecuteJobWithGetInstallationsFailureTest()
    {
        _inventoryJob.Execute(_jobInfrastructureMock.Object);

        var ownInstallation = Assert.Single(_sendDataPoints);
        Assert.True(ownInstallation.HasFields());
        Assert.Contains(_customerInfo.CustomerName, ownInstallation.ToLineProtocol());
        Assert.Contains(_customerInfo.Id.ToString(), ownInstallation.ToLineProtocol());
        Assert.Contains("SelfVersion", ownInstallation.ToLineProtocol());
        Assert.Empty(_jobMsgLog);
    }

    [Fact]
    public void ExecuteJobWithInfluxFailureTest()
    {
        _sendDataPoints = null; // will lead to null reference exception on send...
        _inventoryJob.Execute(_jobInfrastructureMock.Object);

        Assert.NotEmpty(_jobMsgLog);
    }
}