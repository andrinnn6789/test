namespace IAG.Infrastructure.ProcessEngine.Enum;

/// <summary>
///     Enumeration of the result of a job
/// </summary>
public enum JobResultEnum
{
    /// <summary>
    ///     This state should never exist. Is used to have an indicator for uninitialized results
    /// </summary>
    NoResult = 0,

    /// <summary>
    ///     completely successful
    /// </summary>
    Success = 1,

    /// <summary>
    ///     partially successful
    /// </summary>
    PartialSuccess = 10,

    /// <summary>
    ///     failed
    /// </summary>
    Failed = 50,

    /// <summary>
    ///     aborted
    /// </summary>
    Aborted = 60
}