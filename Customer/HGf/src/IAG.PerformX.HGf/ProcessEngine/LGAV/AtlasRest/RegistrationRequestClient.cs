using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public class RegistrationRequestClient : BaseRequestClient<AtlasRegistration>
{
    public RegistrationRequestClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "view/QRegistrationList", logger)
    {
    }
}