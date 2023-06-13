using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public class AttendanceRequestClient : BaseRequestClient<AtlasAttendanceListRegister>
{
    public AttendanceRequestClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "view/QAttendanceListRegister", logger)
    {
    }
}