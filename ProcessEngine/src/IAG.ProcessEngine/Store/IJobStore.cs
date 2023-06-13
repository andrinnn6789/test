using System;
using System.Linq;

using IAG.Infrastructure.Crud;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;

namespace IAG.ProcessEngine.Store;

/// <summary>
///   manages the persistency of jobs.
/// </summary>
public interface IJobStore : ICrud<Guid, IJobState>
{
    /// <summary> Cleanup-task, deletes jobs older than the history time of a job </summary>
    /// <param name="archiveDays"> days to keep completed jobs</param>
    /// <param name="errorDays"> days to keep jobs with errors</param>
    void DeleteOldJobs(int archiveDays, int errorDays);

    /// <summary> Deletes all timed jobs, used in the rebuild of the timed jobs </summary>
    void DeleteScheduledJobs();

    /// <summary> Returns a list of jobs filtered by the parameters </summary>
    /// <returns> List of found jobs </returns>
    IQueryable<IJobState> GetJobs();

    int GetJobCount(JobExecutionStateEnum[] executionStateFilter = null);
}