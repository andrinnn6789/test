using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ObjectMapper;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAG.ProcessEngine.DataLayer.State.ObjectMapper;

public class JobStateFromDbMapper : ObjectMapper<Model.JobState, JobState>
{
    protected override JobState MapToDestination(Model.JobState source, JobState destination)
    {
        return MapFirstToSecond(source, destination, false);
    }
        
    private JobState MapFirstToSecond(Model.JobState source, JobState destination, bool recursion)
    {
        // 1:1
        destination.Id = source.Id;
        destination.TemplateId = source.TemplateId;
        destination.JobConfigId = source.JobConfigId;
        destination.DateCreation = DateTime.SpecifyKind(source.DateCreation, DateTimeKind.Utc);
        destination.DateDue = DateTime.SpecifyKind(source.DateDue, DateTimeKind.Utc);
        destination.IsBlocking = source.IsBlocking;
        destination.MetadataId = source.MetadataId;
        destination.Owner = source.Owner;
        destination.ContextTenant = source.ContextTenant;
        destination.RetryCounter = source.RetryCounter;
        destination.DateRunStart = source.DateRunStart.HasValue ? DateTime.SpecifyKind(source.DateRunStart.Value, DateTimeKind.Utc) : null;
        destination.Progress = source.Progress;
        destination.ExecutionState = source.ExecutionState;
        destination.DateRunEnd = source.DateRunEnd.HasValue ? DateTime.SpecifyKind(source.DateRunEnd.Value, DateTimeKind.Utc) : null;

        // serialized
        if (source.Messages != null)
        {
            destination.Messages = JsonConvert.DeserializeObject<List<MessageStructure>>(source.Messages);
        }
        destination.Messages ??= new List<MessageStructure>();
        foreach (var msg in destination.Messages)
        {
            if (msg.Params == null)
                continue;

            DeserializeParams(msg.Params);
        }
        if (source.ParameterType != null && source.Parameter != null)
        {
            Type paramType = Type.GetType(source.ParameterType);
            destination.Parameter = JsonConvert.DeserializeObject(source.Parameter, paramType!) as IJobParameter;
        }

        if (source.ResultType != null && source.Result != null)
        {
            Type resultType = Type.GetType(source.ResultType);
            destination.Result = JsonConvert.DeserializeObject(source.Result, resultType!) as IJobResult;
        }

        // reconstruct
        if (!recursion)
        {
            if (source.ParentJob != null)
            {
                destination.ParentJob = NewDestination(source.ParentJob);
            }

            if (source.ChildJobs != null)
            {
                destination.ChildJobs = source.ChildJobs
                    .Select(j => (IJobState)MapFirstToSecond(j, new JobState(), true))
                    .ToList();
            }
        }

        destination.Acknowledged = source.Acknowledged;

        return destination;
    }

    private void DeserializeParams(object[] msgParams)
    {
        if (msgParams == null)
            return;

        for (var i = 0; i < msgParams.Length; i++)
        {
            if (msgParams[i] is JObject)
            {
                var localizableParameter = JsonConvert.DeserializeObject<LocalizableParameter>(msgParams[i].ToString() ?? string.Empty);
                msgParams[i] = localizableParameter;
                DeserializeParams(localizableParameter?.Params);
            }
        }
    }
}