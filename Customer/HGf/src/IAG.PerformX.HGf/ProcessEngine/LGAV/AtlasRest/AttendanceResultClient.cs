using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public class AttendanceResultClient : BaseResultClient<AtlasAttendanceResult>
{
    public AttendanceResultClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "LGAVVertragPraesenzmeldung", logger)
    {
    }
}