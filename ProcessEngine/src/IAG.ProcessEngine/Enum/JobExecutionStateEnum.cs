namespace IAG.ProcessEngine.Enum;

/// <summary>
///     Enumeration of the execution states of a job
/// </summary>
public enum JobExecutionStateEnum
{
    /// <summary>
    ///     This state should never exist. Is used to have an indicator for uninitialized jobs
    /// </summary>
    Undefined = 0,

    /// <summary>
    ///     new and runnable job
    /// </summary>
    New = 10,

    /// <summary>
    ///     job is waiting for execution
    /// </summary>
    Scheduled = 20,

    /// <summary>
    ///     job is running
    /// </summary>
    Running = 30,

    /// <summary>
    ///     job is finished
    /// </summary>
    Success = 40,

    /// <summary>
    ///     job is finished, but result has errors
    /// </summary>
    Warning = 45,

    /// <summary>
    ///     Execution failed
    /// </summary>
    Failed = 50,

    /// <summary>
    ///     Job has been aborted
    /// </summary>
    Aborted = 60
}