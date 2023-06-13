using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Resource;

using Microsoft.Extensions.Localization;

namespace IAG.ProcessEngine.Execution.Model;

[ExcludeFromCodeCoverage]
public class JobLogLocalized
{
    private readonly IJobState _jobState;
        
    public JobLogLocalized(IJobState jobState, IStringLocalizer localizer)
    {
        _jobState = jobState;
        var msgLocalizer = new MessageLocalizer(localizer);
        LocalizedMessages = msgLocalizer.Localize(jobState.Messages);
        ExecutionState = localizer.GetString($"{ResourceIds.ResourcePrefix}{_jobState.ExecutionState}");
        Result = new TypeLocalizer(localizer).Localize(ResourceIds.ResourcePrefix, _jobState.Result);
    }

    public Guid Id => _jobState.Id;

    public List<MessageStructureLocalized> LocalizedMessages { get; }

    public Guid JobConfigId => _jobState.JobConfigId;

    public DateTime DateCreation => _jobState.DateCreation;

    public DateTime DateDue => _jobState.DateDue;

    public string Owner => _jobState.Owner;

    public int RetryCounter => _jobState.RetryCounter;

    public IJobParameter Parameter => _jobState.Parameter;

    public DateTime? DateRunStart => _jobState.DateRunStart;

    public double Progress => _jobState.Progress;

    public DateTime? DateRunEnd => _jobState.DateRunEnd;

    public string ExecutionState { get; }

    public string Result { get; }
};