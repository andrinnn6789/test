using IAG.InstallClient.BusinessLogic.Model;

namespace IAG.InstallClient.BusinessLogic;

public interface IServiceManager
{
    string GetServiceName(string executablePath);
    string InstallService(string productName, string instanceName);
    void UninstallService(string serviceName);
    ServiceStatus? GetServiceState(string serviceName);
    void StartService(string serviceName);
    void StopService(string serviceName);
}