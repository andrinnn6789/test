namespace IAG.ProcessEngine.Execution;

public interface IScheduler
{
    event JobBeforeEnqueueEventHandler OnBeforeEnqueueJob;

    bool IsRunning { get; }
        
    void Start();

    void Stop();
}