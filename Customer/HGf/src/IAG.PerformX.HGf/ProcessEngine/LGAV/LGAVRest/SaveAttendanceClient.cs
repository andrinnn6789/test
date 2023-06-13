using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;

public class SaveAttendanceClient : BaseClient<AttendanceListRegisterMainObject, AttendanceListRegisterResponseMainObject>
{
    public SaveAttendanceClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "saveattendances", logger)
    {
    }
}