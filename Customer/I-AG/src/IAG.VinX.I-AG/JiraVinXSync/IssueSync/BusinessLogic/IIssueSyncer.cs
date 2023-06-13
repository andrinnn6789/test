using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;

public interface IIssueSyncer
{
    public void SetConfig(
        string sybaseConnectionString,
        HttpConfig httpConfig,
        IMessageLogger messageLogger);
        
    public IJobResult SyncIssues();
}