using System;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.InstallClient.BusinessLogic;

public interface IInventoryManager
{
    Task<InstallationInfo> RegisterInstallationAsync(Guid customerId, InstallationRegistration installationRegistration);
    Task<InstallationInfo> DeRegisterInstallationAsync(Guid customerId, string instanceName);
}