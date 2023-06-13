
namespace IAG.ProcessEngine.Store.Model;

public interface IEngineConfig
{
    /// <summary>
    /// time in milliseconds to wait for all jobs to shutdown
    /// </summary>
    int JobShutdownDelay { get; set; }
}