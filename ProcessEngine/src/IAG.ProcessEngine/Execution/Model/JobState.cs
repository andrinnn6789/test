using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Enum;

namespace IAG.ProcessEngine.Execution.Model;

public class JobState : IJobState
{
    public JobState()
    {
        Id = Guid.NewGuid();
        ChildJobs = new List<IJobState>();
        ExecutionState = JobExecutionStateEnum.New;
        DateCreation = DateTime.UtcNow;
        DateDue = DateTime.MaxValue;
        Messages = new List<MessageStructure>();
    }
        
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public Guid JobConfigId { get; set; }

    public IJobState ParentJob { get; set; }

    public List<IJobState> ChildJobs { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime DateDue { get; set; }

    public bool IsBlocking { get; set; }

    public Guid MetadataId { get; set; }

    public string Owner { get; set; }

    public Guid? ContextTenant { get; set; }

    public int RetryCounter { get; set; }

    public IJobParameter Parameter { get; set; }

    public DateTime? DateRunStart { get; set; }

    [NotMapped]
    public DateTime? LastHeartbeat { get; set; }

    public double Progress { get; set; }

    public DateTime? DateRunEnd { get; set; }

    public JobExecutionStateEnum ExecutionState { get; set; }

    public List<MessageStructure> Messages { get; set; }

    public IJobResult Result { get; set; }

    public bool Acknowledged { get; set; }
}