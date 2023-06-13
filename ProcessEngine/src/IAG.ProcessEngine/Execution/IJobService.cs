using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution.Model;

namespace IAG.ProcessEngine.Execution;

public interface IJobService
{
    /// <summary>
    ///     Gets the running state of the service
    /// </summary>
    bool Running { get; }

    /// <summary>
    ///     Gets a list of running jobs
    /// </summary>
    IEnumerable<IJobInstance> RunningJobs { get; }

    /// <summary>
    ///     Creates a new job instance from the template id
    /// </summary>
    /// <param name="jobTemplateId">id of the job instance</param>
    /// <returns>job instance</returns>
    IJobParameter GetJobParameter(Guid jobTemplateId);

    /// <summary>
    ///     Creates a new job instance from the job config id
    /// </summary>
    /// <param name="jobConfigId">id of the job config</param>
    /// <param name="jobStateParent">Instance-state of parent-job</param>
    /// <returns>job instance</returns>
    IJobInstance CreateJobInstance(Guid jobConfigId, IJobState jobStateParent = null);

    /// <summary>
    ///     Creates a new job instance from a job state
    /// </summary>
    /// <param name="jobState">state of the job instance</param>
    /// <returns>job instance</returns>
    IJobInstance CreateJobInstance(IJobState jobState);

    /// <summary>
    ///     Enqueues a job
    /// </summary>
    /// <param name="jobInstance">job to enqueue</param>
    /// <returns>Task object for runtime control</returns>
    Task EnqueueJob(IJobInstance jobInstance);

    /// <summary>
    /// Gets information about a job instance
    /// </summary>
    /// <param name="jobInstanceId">id of the job instance</param>
    /// <returns>state of the job instance</returns>
    IJobState GetJobInstanceState(Guid jobInstanceId);

    /// <summary>
    ///     Aborts an enqueued job instance
    /// </summary>
    /// <param name="jobInstanceId">id of the job instance to abort</param>
    /// <returns>true if the job instance has been successfully informed about the wish to cancel it. False otherwise (e.g. job instance with the given jobInstanceId not found)</returns>
    bool AbortJob(Guid jobInstanceId);
        
    /// <summary>
    ///     Starts the service
    /// </summary>
    void Start();

    /// <summary>
    ///     Stops the service
    /// </summary>
    void Stop();
}