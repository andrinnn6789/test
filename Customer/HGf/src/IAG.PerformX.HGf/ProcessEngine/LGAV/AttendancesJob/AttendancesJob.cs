using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;
using IAG.PerformX.HGf.Resource;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;

[JobInfo("6F4FC02B-7902-436B-B2B6-543B0003B7FD", JobName)]
public class AttendancesJob : JobBase<AttendancesConfig, JobParameter, AttendancesResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "Attendances";

    private readonly IRequestResponseLogger _requestLogger;
    private AttendanceMapper _attendanceMapper;
    private AttendanceResultMapper _attendanceResultMapper;
    private AttendanceRequestClient _attendanceRequestClient;
    private AttendanceResultClient _attendanceResultClient;
    private SaveAttendanceClient _saveAttendanceClient;

    public AttendancesJob(ILogger<AttendancesJob> logger)
    {
        _requestLogger = new RequestResponseLogger(logger);
    }

    protected override void ExecuteJob()
    {
        _attendanceMapper = new AttendanceMapper(Config.AtlasBasePath);
        _attendanceResultMapper = new AttendanceResultMapper();
        _attendanceRequestClient = new AttendanceRequestClient(new AtlasConfig(Config.AtlasCredentials), _requestLogger);
        _attendanceResultClient = new AttendanceResultClient(new AtlasConfig(Config.AtlasCredentials), _requestLogger);
        _saveAttendanceClient = new SaveAttendanceClient(Config.LgavConfig, _requestLogger);
            
        var atlasAttendances = GetAtlasAttendances().ToList();
        HeartbeatAndCheckCancellation();

        var lgavAttendanceList = MapEvents(atlasAttendances);
        HeartbeatAndCheckCancellation();

        if (lgavAttendanceList.AttendanceListRegister.AttendanceListRegisterItems.Count > 0)
        {
            var lgavResults = SaveRegistrations(lgavAttendanceList);
            Heartbeat();
            WriteResults(lgavResults, atlasAttendances);
        }

        if (Result.ErrorCount > 0)
        {
            Result.Result = Result.SuccessfulWriteResultCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }

    private IEnumerable<AtlasAttendanceListRegister> GetAtlasAttendances()
    {
        List<AtlasAttendanceListRegister> atlasAttendances;
        try
        {
            atlasAttendances = _attendanceRequestClient.Get().Result.ToList();
            Result.AtlasAttendancesCount = atlasAttendances.Count;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.AttendancesGetFromAtlasErrorFormatMessage, ex);
        }

        return atlasAttendances;
    }

    private AttendanceListRegisterMainObject MapEvents(IEnumerable<AtlasAttendanceListRegister> atlasAttendances)
    {
        var lgavAttendanceList = new AttendanceListRegisterMainObject();
        lgavAttendanceList.AttendanceListRegister.Header.ApiKey = Config.LgavApiKey;
        lgavAttendanceList.AttendanceListRegister.Header.TransactionDate = DateTime.Now.ToString("s");

        foreach (var atlasAttendance in atlasAttendances)
        {
            try
            {
                var lgavAttendance = _attendanceMapper.NewDestination(atlasAttendance);
                lgavAttendanceList.AttendanceListRegister.AttendanceListRegisterItems.Add(lgavAttendance);
            }
            catch (Exception ex)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.AttendancesMapToLgavErrorFormatMessage, JsonConvert.SerializeObject(atlasAttendance), LocalizableException.GetExceptionMessage(ex));
                AddMessage(ex);
            }
        }

        return lgavAttendanceList;
    }

    private AttendanceListRegisterResponseMainObject SaveRegistrations(AttendanceListRegisterMainObject lgavAttendanceList)
    {
        AttendanceListRegisterResponseMainObject lgavResults;
        try
        {
            lgavResults = _saveAttendanceClient.Post(lgavAttendanceList).Result;
            Result.LgavResultAttendancesCount = lgavResults.AttendanceListRegisterResponseItems.Count;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.AttendancesWriteToLgavErrorFormatMessage, ex);
        }

        return lgavResults;
    }

    private void WriteResults(AttendanceListRegisterResponseMainObject lgavResults, IEnumerable<AtlasAttendanceListRegister> atlasAttendances)
    {
        var atlasAttendancesList = atlasAttendances.ToList();

        foreach (var lgavResult in lgavResults.AttendanceListRegisterResponseItems)
        {
            var atlasRegister = atlasAttendancesList.FirstOrDefault(x => x.AttendanceListUid == lgavResult.AttendanceListUid);

            if (atlasRegister == null) continue;
            foreach (var atlasAttendance in atlasRegister.AttendanceListItems)
            {
                var atlasResult = _attendanceResultMapper.NewDestination(lgavResult);

                try
                {
                    _attendanceResultClient.Put(atlasResult, atlasAttendance.Id).Wait();
                    Result.SuccessfulWriteResultCount++;
                    Heartbeat();
                }
                catch (Exception ex)
                {
                    AddMessage(MessageTypeEnum.Error, ResourceIds.AttendancesWriteResultErrorFormatMessage, JsonConvert.SerializeObject(lgavResult));
                    AddMessage(ex);
                    Result.ErrorCount++;
                }
            }
        }
    }
}