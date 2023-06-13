using System.Threading.Tasks;

using IAG.Infrastructure.ProcessEngine.Enum;

namespace IAG.VinX.Smith.HelloTess.SyncLogic;

public interface ISyncHandler
{
    Task DoSync();

    JobResultEnum CheckSyncJobResult();
}