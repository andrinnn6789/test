namespace IAG.Infrastructure.Globalisation.Enum;

/// <summary>
///     Little categorization for messages
/// </summary>
public enum MessageTypeEnum
{
    /// <summary>
    ///     Information info
    /// </summary>
    Information = 1,

    /// <summary>
    ///     Description info
    /// </summary>
    Description = 3,

    /// <summary>
    ///     Indicates the success of an operation
    /// </summary>
    Success = 5,

    /// <summary>
    ///     Summary of the operation for the user
    /// </summary>
    Summary = 6,

    /// <summary>
    ///     Debug-info
    /// </summary>
    Debug = 7,

    /// <summary>
    ///     Warning-Info
    /// </summary>
    Warning = 8,

    /// <summary>
    ///     Error-Message
    /// </summary>
    Error = 9
}