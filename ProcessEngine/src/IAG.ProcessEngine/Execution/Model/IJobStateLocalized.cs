using System;
using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Enum;

namespace IAG.ProcessEngine.Execution.Model;

/// <summary>
///     Interface of a the state of a localized job
/// </summary>
public interface IJobStateLocalized
{
    /// <summary>
    ///     TemplateId of the job
    /// </summary>
    Guid TemplateId { get; }

    /// <summary>
    ///     TemplateId of the job
    /// </summary>
    Guid JobConfigId { get; }

    /// <summary>
    ///     if the job has a master job/parent job its id is returned here
    /// </summary>
    IJobState ParentJob { get; }

    /// <summary>
    ///     List of all childs-jobs, can be empty
    /// </summary>
    List<IJobState> ChildJobs { get; }

    /// <summary>
    ///     Date/time of creation
    /// </summary>
    DateTime DateCreation { get; }

    /// <summary>
    ///     Date/time of scheduled execution
    /// </summary>
    DateTime DateDue { get; }

    /// <summary>
    ///     returns true if the job is called in blocking mode
    /// </summary>
    bool IsBlocking { get; }

    /// <summary>
    ///     metadata of the job
    /// </summary>
    Guid MetadataId { get; }

    /// <summary>
    ///     Owner of the job with informational purpose
    /// </summary>
    string Owner { get; }

    /// <summary>
    ///     current retries
    /// </summary>
    int RetryCounter { get; }

    /// <summary>
    /// Parameters for the job
    /// </summary>
    IJobParameter Parameter { get; }

    /// <summary>
    ///     Date/time of execution start
    /// </summary>
    DateTime? DateRunStart { get; }

    /// <summary>
    ///     Progress from 0 - 100
    /// </summary>
    double Progress { get; }

    /// <summary>
    ///     Date/time of execution end
    /// </summary>
    DateTime? DateRunEnd { get; }

    /// <summary>
    ///     <see cref="JobExecutionStateEnum" />
    /// </summary>
    JobExecutionStateEnum ExecutionState { get; }

    /// <summary>
    ///     List of messages from the job in a localized format
    /// </summary>
    List<MessageStructureLocalized> LocalizedMessages { get; }

    /// <summary>
    /// Result of the job execution
    /// </summary>
    IJobResult Result { get; }
}