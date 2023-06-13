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
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;
using IAG.PerformX.HGf.Resource;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;

[JobInfo("AC180EC8-4961-4E5A-AC95-2B9683215206", JobName)]
public class EventsJob : JobBase<EventsConfig, JobParameter, EventsResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "Events";

    private readonly IRequestResponseLogger _requestLogger;
    private EventMapper _eventMapper;
    private EventResultMapper _eventResultMapper;
    private EventRequestClient _eventRequestClient;
    private EventResultClient _eventResultClient;
    private SaveEventsClient _saveEventsClient;

    public EventsJob(ILogger<EventsJob> logger)
    {
        _requestLogger = new RequestResponseLogger(logger);
    }

    protected override void ExecuteJob()
    {
        _eventMapper = new EventMapper();
        _eventResultMapper = new EventResultMapper();
        _eventRequestClient = new EventRequestClient(new AtlasConfig(Config.AtlasCredentials), _requestLogger);
        _eventResultClient = new EventResultClient(new AtlasConfig(Config.AtlasCredentials), _requestLogger);
        _saveEventsClient = new SaveEventsClient(Config.LgavConfig, _requestLogger);
            
        var atlasEvents = GetAtlasEvents().ToList();
        HeartbeatAndCheckCancellation();

        var lgavEventList = MapEvents(atlasEvents);
        HeartbeatAndCheckCancellation();

        if (lgavEventList.EventList.EventListItems.Count > 0)
        {
            var lgavResults = SaveEvents(lgavEventList);
            Heartbeat();
            WriteResults(lgavResults);
        }

        if (Result.ErrorCount > 0)
        {
            Result.Result = Result.SuccessfulWriteResultCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }

    private IEnumerable<AtlasEvent> GetAtlasEvents()
    {
        List<AtlasEvent> atlasEvents;
        try
        {
            atlasEvents = _eventRequestClient.Get().Result.ToList();
            Result.AtlasEventsCount = atlasEvents.Count;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.EventsGetFromAtlasErrorFormatMessage, ex);
        }

        return atlasEvents;
    }

    private EventListMainObject MapEvents(IEnumerable<AtlasEvent> atlasEvents)
    {
        var lgavEventList = new EventListMainObject();
        lgavEventList.EventList.Header.ApiKey = Config.LgavApiKey;
        lgavEventList.EventList.Header.TransactionDate = DateTime.Now.ToString("s");

        foreach (var atlasEvent in atlasEvents)
        {
            try
            {
                var lgavEvent = _eventMapper.NewDestination(atlasEvent);
                lgavEventList.EventList.EventListItems.Add(lgavEvent);
            }
            catch (Exception ex)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.EventsMapToLgavErrorFormatMessage, JsonConvert.SerializeObject(atlasEvent), LocalizableException.GetExceptionMessage(ex));
                AddMessage(ex);
            }
        }

        return lgavEventList;
    }

    private EventListResponseMainObject SaveEvents(EventListMainObject lgavEventList)
    {
        EventListResponseMainObject lgavResults;
        try
        {
            lgavResults = _saveEventsClient.Post(lgavEventList).Result;
            Result.LgavResultEventsCount = lgavResults.EventListResponseItems.Count;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.EventsWriteToLgavErrorFormatMessage, ex);
        }

        return lgavResults;
    }

    private void WriteResults(EventListResponseMainObject lgavResults)
    {
        foreach (var lgavResult in lgavResults.EventListResponseItems)
        {
            var atlasResult = _eventResultMapper.NewDestination(lgavResult);

            try
            {
                _eventResultClient.Put(atlasResult, lgavResult.EventUid).Wait();
                Result.SuccessfulWriteResultCount++;
                Heartbeat();
            }
            catch (Exception ex)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.EventsWriteResultErrorFormatMessage, JsonConvert.SerializeObject(lgavResult));
                AddMessage(ex);
                Result.ErrorCount++;
            }
        }
    }
}