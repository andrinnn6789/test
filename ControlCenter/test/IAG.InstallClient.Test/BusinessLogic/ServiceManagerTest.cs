using System;

using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

public class ServiceManagerTest
{
    [Fact(Skip = "Can just be tested on Windows...")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "see skip")]
    public void BasicFunctionTest()
    {
        var serviceManager = new ServiceManager();

        var serviceName = serviceManager.GetServiceName(Environment.SystemDirectory);
        var serviceState = serviceManager.GetServiceState("EventLog");

        Assert.NotEmpty(serviceName);
        Assert.NotNull(serviceState);
        Assert.Equal(ServiceStatus.Running, serviceState.Value);
    }
}