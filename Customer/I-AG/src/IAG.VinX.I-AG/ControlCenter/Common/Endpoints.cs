using IAG.ControlCenter;
using IAG.Infrastructure.Controllers;

namespace IAG.VinX.IAG.ControlCenter.Common;

public static class Endpoints
{
    public const string Resource = InfrastructureEndpoints.Resource + "ResourceAdmin/";

    public const string Control = ControlCenterEndpoints.Mobile + "LicenceSync/";
       
    public const string Distribution = ControlCenterEndpoints.Distribution;
}