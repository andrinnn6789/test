using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.BusinessLogic;

public interface IVersionSyncer
{
    void SetConfig(string sybaseConnectionString, HttpConfig httpConfig, IMessageLogger messageLogger);

    IJobResult SyncVersions();
}