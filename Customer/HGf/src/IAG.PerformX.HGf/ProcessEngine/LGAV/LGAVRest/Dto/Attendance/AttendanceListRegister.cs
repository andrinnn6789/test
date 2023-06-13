using System.Collections.Generic;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

public class AttendanceListRegister
{
    public AttendanceListRegister()
    {
        Header = new Header();
        AttendanceListRegisterItems = new List<AttendanceListRegisterItem>();
    }

    public Header Header { get; set; }

    public List<AttendanceListRegisterItem> AttendanceListRegisterItems { get; set; }
}