using System;
using System.Collections.Generic;

using IAG.Infrastructure.DataLayer.Model.Base;
using IAG.ProcessEngine.Enum;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.DataLayer.State.Model;

public class JobState : BaseEntity
{
    public JobState()
    {
        ChildJobIds = new List<Guid>();
        ChildJobs = new List<JobState>();
    }

    public Guid? ParentJobId { get; set; }

    public ICollection<Guid> ChildJobIds { get; }
        
    public Guid TemplateId { get; set; }

    public Guid JobConfigId { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime DateDue { get; set; }

    public bool IsBlocking { get; set; }

    public Guid MetadataId { get; set; }

    public string Owner { get; set; }

    public Guid? ContextTenant { get; set; }

    public int RetryCounter { get; set; }

    public string ParameterType { get; set; }

    public string Parameter { get; set; }

    public DateTime? DateRunStart { get; set; }

    public double Progress { get; set; }

    public JobExecutionStateEnum ExecutionState { get; set; }

    public DateTime? DateRunEnd { get; set; }

    public string Messages { get; set; }

    public string ResultType { get; set; }

    public string Result { get; set; }
        
    public bool Acknowledged { get; set; }

    [UsedImplicitly]
    public ICollection<JobState> ChildJobs { get; set; }

    [UsedImplicitly]
    public JobState ParentJob { get; set; }
}