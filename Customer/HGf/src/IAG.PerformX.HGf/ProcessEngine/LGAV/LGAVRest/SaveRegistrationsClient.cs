using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;

public class SaveRegistrationsClient : BaseClient<RegistrationListMainObject, RegistrationListResponseMainObject>
{
    public SaveRegistrationsClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "saveregistrations", logger)
    {
    }
}