using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.ControlCenter.Config;

namespace IAG.VinX.IAG.ControlCenter.Common;

public interface IControlCenterTokenRequest
{
    IHttpConfig GetConfig(BackendConfig backendConfig, string endpoint, IRequestResponseLogger logger);
}