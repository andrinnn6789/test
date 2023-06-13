using System;
using System.IO;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.ObjectMapper;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;
using IAG.PerformX.HGf.Resource;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;

public class AttendanceMapper : ObjectMapper<AtlasAttendanceListRegister, AttendanceListRegisterItem>
{
    private readonly string _fileBasePath;

    public AttendanceMapper(string atlasBasePath)
    {
        if (!string.IsNullOrEmpty(atlasBasePath))
        {
            _fileBasePath = atlasBasePath.EndsWith("\\") ? atlasBasePath.Substring(0, atlasBasePath.Length - 1) : atlasBasePath;
        }
    }

    protected override AttendanceListRegisterItem MapToDestination(AtlasAttendanceListRegister source, AttendanceListRegisterItem destination)
    {
        destination.Mode = "upsert";
        destination.AttendanceListUid = source.AttendanceListUid;

        var atlasFirstAttendanceListItem = source.AttendanceListItems.FirstOrDefault();
        MapEventId(atlasFirstAttendanceListItem, destination);

        MapAttendanceDates(source, destination);
        MapAttendanceItems(source, destination);
        MapDocuments(source, destination);

        return destination;
    }

    private void MapEventId(AtlasAttendanceListItem atlasFirstAttendanceListItem, AttendanceListRegisterItem attendanceListRegisterItem)
    {
        if (atlasFirstAttendanceListItem != null)
        {
            attendanceListRegisterItem.EventUid = atlasFirstAttendanceListItem.EventUid;
        }
    }

    private void MapAttendanceDates(AtlasAttendanceListRegister atlasAttendanceList, AttendanceListRegisterItem attendanceListRegisterItem)
    {
        attendanceListRegisterItem.AttendanceListDates.AttendanceListBegin = atlasAttendanceList.AttendanceListBegin?.ToString("s");
        attendanceListRegisterItem.AttendanceListDates.AttendanceListEnd = atlasAttendanceList.AttendanceListEnd?.ToString("s");
    }

    private void MapAttendanceItems(AtlasAttendanceListRegister atlasAttendanceList, AttendanceListRegisterItem attendanceListRegisterItem)
    {
        foreach (var atlasAttendance in atlasAttendanceList.AttendanceListItems)
        {
            var attendanceListItem = new AttendanceListItem();
            attendanceListItem.Mode = "upsert";
            attendanceListItem.RegistrationUid = atlasAttendance.RegistrationUid;
            attendanceListItem.Attendance.NumberOfDays = atlasAttendance.NumberOfDays;
            attendanceListItem.Attendance.Notes = atlasAttendance.Notes;

            attendanceListRegisterItem.AttendanceListItems.Add(attendanceListItem);
        }
    }

    private void MapDocuments(AtlasAttendanceListRegister atlasAttendanceList, AttendanceListRegisterItem attendanceListRegisterItem)
    {
        if (!string.IsNullOrEmpty(atlasAttendanceList.Document))
        {
            try
            {
                var fullPath = FormatFilePath(atlasAttendanceList.Document);

                var bytes = File.ReadAllBytes(fullPath);
                var base64String = Convert.ToBase64String(bytes);
                attendanceListRegisterItem.Documents.Add(base64String);
            }
            catch (Exception ex)
            {
                throw new LocalizableException(ResourceIds.AttendancesLoadFileErrorFormatMessage, ex, atlasAttendanceList.Document);
            }
        }
    }

    private string FormatFilePath(string relativePath)
    {
        if (relativePath.StartsWith("."))
        {
            relativePath = relativePath.Substring(1);
        }

        return _fileBasePath + relativePath;
    }
}