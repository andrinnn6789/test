using System.Threading;

namespace IAG.Infrastructure.ProcessEngine.Execution;

public interface IJobHeartbeatObserver
{
    CancellationToken JobCancellationToken { get; }

    void Heartbeat();

    void HeartbeatAndCheckJobCancellation();
}