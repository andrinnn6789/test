using System;

using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.ProcessEngine.Execution.Model;

public interface IJobInstance : IDisposable
{
    /// <summary>
    /// Id of the job instance
    /// </summary>
    Guid Id { get; }
        
    /// <summary>
    ///     Gets the state of the job
    /// </summary>
    IJobState State { get; }

    /// <summary>
    ///     Gets the job
    /// </summary>
    IJob Job { get; }

    IServiceProvider ServiceProvider { get; }
}