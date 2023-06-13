namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

public class AttendanceListRegisterMainObject
{
    public AttendanceListRegisterMainObject()
    {
        AttendanceListRegister = new AttendanceListRegister();
    }

    public AttendanceListRegister AttendanceListRegister { get; set; }
}