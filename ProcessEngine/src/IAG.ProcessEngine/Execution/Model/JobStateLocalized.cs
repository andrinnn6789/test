using System;
using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Enum;

namespace IAG.ProcessEngine.Execution.Model;

public class JobStateLocalized : IJobStateLocalized
{
    private readonly IJobState _jobState;
        
    public JobStateLocalized(IJobState jobState, IMessageLocalizer localizer)
    {
        _jobState = jobState;
        LocalizedMessages = localizer.Localize(jobState.Messages);
    }

    public Guid Id => _jobState.Id;

    public List<MessageStructureLocalized> LocalizedMessages { get; }

    public Guid TemplateId => _jobState.TemplateId;

    public Guid JobConfigId => _jobState.JobConfigId;

    public IJobState ParentJob => _jobState.ParentJob;

    public List<IJobState> ChildJobs => _jobState.ChildJobs;

    public DateTime DateCreation => _jobState.DateCreation;

    public DateTime DateDue => _jobState.DateDue;

    public bool IsBlocking => _jobState.IsBlocking;

    public Guid MetadataId => _jobState.MetadataId;

    public string Owner => _jobState.Owner;

    public Guid? Tenant => _jobState.ContextTenant;

    public int RetryCounter => _jobState.RetryCounter;

    public IJobParameter Parameter => _jobState.Parameter;

    public DateTime? DateRunStart => _jobState.DateRunStart;

    public double Progress => _jobState.Progress;

    public DateTime? DateRunEnd => _jobState.DateRunEnd;

    public JobExecutionStateEnum ExecutionState => _jobState.ExecutionState;

    public IJobResult Result => _jobState.Result;
}