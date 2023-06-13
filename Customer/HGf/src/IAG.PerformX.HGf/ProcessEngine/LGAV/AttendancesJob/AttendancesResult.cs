using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;

public class AttendancesResult : JobResult
{
    public int AtlasAttendancesCount { get; set; }

    public int LgavResultAttendancesCount { get; set; }

    public int SuccessfulWriteResultCount { get; set; }
}