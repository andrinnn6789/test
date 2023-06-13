using System.Collections.Generic;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

public class AttendanceListRegisterResponseMainObject
{
    public AttendanceListRegisterResponseMainObject()
    {
        AttendanceListRegisterResponseItems = new List<AttendanceListRegisterResponseItem>();
    }

    public List<AttendanceListRegisterResponseItem> AttendanceListRegisterResponseItems { get; set; }
}