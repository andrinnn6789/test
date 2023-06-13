using System;

using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.BusinessLogic;

public interface IWorklogSyncer
{
    void SetConfig(string sybaseConnectionString, HttpConfig httpConfig, IMessageLogger messageLogger);

    IJobResult SyncWorklogs(DateTime lastSync);
}