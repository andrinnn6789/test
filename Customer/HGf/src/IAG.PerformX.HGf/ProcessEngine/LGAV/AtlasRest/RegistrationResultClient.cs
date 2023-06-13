using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public class RegistrationResultClient : BaseResultClient<AtlasRegistrationResult>
{
    public RegistrationResultClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "LGAVAntrag", logger)
    {
    }
}