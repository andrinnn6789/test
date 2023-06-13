namespace IAG.ProcessEngine.Execution;

public interface IMonitor
{
    bool IsRunning { get; }

    void Start();

    void Stop();
}