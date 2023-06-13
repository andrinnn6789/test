using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store.Model;

namespace IAG.ProcessEngine.Execution;

/// <summary>
///     Interface to the executer that executes jobs
/// </summary>
public interface IJobExecuter : IDisposable
{
    IEngineConfig Config { get; set; }
        
    /// <summary>
    ///     Gets the running state of the executer
    /// </summary>
    bool Running { get; }

    /// <summary>
    ///     Gets a list of running jobs
    /// </summary>
    IEnumerable<IJobInstance> RunningJobs { get; }

    /// <summary>
    ///     Start the executer
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the executer
    /// </summary>
    void Stop();

    /// <summary>
    ///     Add a job instance to execute (asynchronous)
    /// </summary>
    /// <param name="job">job instance to enqueue for execution</param>
    /// <param name="jobService">job service to serve the job's desires</param>
    /// <returns>Task object of the running job instance</returns>
    Task EnqueueJob(IJobInstance job, IJobService jobService);

    /// <summary>
    /// Gets information about a job instance
    /// </summary>
    /// <param name="jobInstanceId">id of the job instance</param>
    /// <returns>state of the job instance</returns>
    IJobState GetJobState(Guid jobInstanceId);

    /// <summary>
    ///     Cancels a running job instance
    /// </summary>
    /// <param name="jobInstanceId">jobInstanceId of the job instance</param>
    /// <returns>true if the job instance has been successfully informed about the wish to cancel it. False otherwise (e.g. job instance with the given jobInstanceId not found)</returns>
    bool CancelJob(Guid jobInstanceId);
}