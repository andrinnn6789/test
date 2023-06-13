using IAG.Infrastructure.ObjectMapper;
using IAG.InstallClient.BusinessLogic.Model;

namespace IAG.InstallClient.Models.Mapper;

public class InstallationViewModelMapper : ObjectMapper<InstalledRelease, InstallationViewModel>
{
    protected override InstallationViewModel MapToDestination(InstalledRelease source, InstallationViewModel destination)
    {
        destination.InstanceName = source.InstanceName;
        destination.ProductName = source.ProductName;
        destination.Version = source.Version;
        destination.CustomerPluginName = source.CustomerPluginName;

        return destination;
    }
}