using System;

using IAG.Infrastructure.ObjectMapper;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;

public class AttendanceResultMapper : ObjectMapper<AttendanceListRegisterResponseItem, AtlasAttendanceResult>
{
    protected override AtlasAttendanceResult MapToDestination(AttendanceListRegisterResponseItem source, AtlasAttendanceResult destination)
    {
        destination.HgfLgavBestaetigung = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        destination.HgfLgavId = source.AttendanceListId;

        return destination;
    }
}