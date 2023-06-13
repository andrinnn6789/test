using IAG.Infrastructure.ObjectMapper;
using IAG.ProcessEngine.Execution.Model;

using Newtonsoft.Json;

using JobState = IAG.ProcessEngine.DataLayer.State.Model.JobState;

namespace IAG.ProcessEngine.DataLayer.State.ObjectMapper;

public class JobStateToDbMapper : ObjectMapper<IJobState, JobState>
{
    protected override JobState MapToDestination(IJobState source, JobState destination)
    {
        // 1:1
        destination.Id = source.Id;
        destination.TemplateId = source.TemplateId;
        destination.JobConfigId = source.JobConfigId;
        destination.ParentJobId = source.ParentJob?.Id;
        destination.DateCreation = source.DateCreation;
        destination.DateDue = source.DateDue;
        destination.IsBlocking = source.IsBlocking;
        destination.MetadataId = source.MetadataId;
        destination.Owner = source.Owner;
        destination.ContextTenant = source.ContextTenant;
        destination.RetryCounter = source.RetryCounter;
        destination.DateRunStart = source.DateRunStart;
        destination.Progress = source.Progress;
        destination.ExecutionState = source.ExecutionState;
        destination.DateRunEnd = source.DateRunEnd;
        foreach (var childJob in source.ChildJobs)
        {
            destination.ChildJobIds.Add(childJob.Id);
        }

        // serialize
        destination.Messages = source.Messages != null ? JsonConvert.SerializeObject(source.Messages) : null;
        if (source.Parameter != null)
        {
            destination.ParameterType = source.Parameter.GetType().AssemblyQualifiedName;
            destination.Parameter = JsonConvert.SerializeObject(source.Parameter);
        }
        else
        {
            destination.ParameterType = null;
            destination.Parameter = null;
        }

        if (source.Result != null)
        {
            destination.ResultType = source.Result.GetType().AssemblyQualifiedName;
            destination.Result = JsonConvert.SerializeObject(source.Result);
        }
        else
        {
            destination.ResultType = null;
            destination.Result = null;
        }

        destination.Acknowledged = source.Acknowledged;

        return destination;
    }
}