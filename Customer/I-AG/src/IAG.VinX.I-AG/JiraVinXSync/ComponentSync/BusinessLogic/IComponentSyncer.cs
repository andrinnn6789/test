using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.BusinessLogic;

public interface IComponentSyncer
{
    void SetConfig(string sybaseConnectionString, HttpConfig httpConfig, IMessageLogger messageLogger);

    IJobResult SyncComponents();
}