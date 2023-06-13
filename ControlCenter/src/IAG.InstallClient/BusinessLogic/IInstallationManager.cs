using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Logging;
using IAG.InstallClient.BusinessLogic.Model;

namespace IAG.InstallClient.BusinessLogic;

public interface IInstallationManager
{
    string CurrentSelfVersion { get; }

    Task<IEnumerable<InstalledRelease>> GetInstallationsAsync();
    Task<string> CreateOrUpdateInstallationAsync(InstallationSetup setup, IMessageLogger messageLogger = null);
    void TransferInstance(string sourceInstance, string targetInstance, IMessageLogger messageLogger = null);
    void DeleteInstance(string instanceName);
    void DeleteInstanceDirectory(string instanceName);
    Task DoSelfUpdate(InstallationSetup setup, IMessageLogger messageLogger = null);
}