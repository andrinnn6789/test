using System;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.ProcessEngine;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;

public class IssueSyncer : IIssueSyncer
{
    private readonly IIssueSyncVinXConnector _vinXConnector;
        
    private IssueSyncResult _result;
    private IMessageLogger _messageLogger;
    private JiraIssueClient _issueClient;

    public IssueSyncer(IIssueSyncVinXConnector vinXConnector)
    {
        _vinXConnector = vinXConnector;
    }

    public void SetConfig(
        string sybaseConnectionString,
        HttpConfig httpConfig,
        IMessageLogger messageLogger)
    {
        _vinXConnector.CreateConnection(sybaseConnectionString);
        var requestLogger = new RequestResponseLogger(_messageLogger);
        _issueClient = new JiraIssueClient(httpConfig, requestLogger);
        _messageLogger = messageLogger;

    }

    public IJobResult SyncIssues()
    {
        _result = new IssueSyncResult
        {
            Result = JobResultEnum.Success
        };

        _vinXConnector.InitPendenzSyncSettings();
        _vinXConnector.InitPendenzSyncDetailSettings();
        _vinXConnector.InitMapper(_messageLogger);

        var lastSync = _vinXConnector.GetLastSync();
        var jiraIssues = _vinXConnector.LoadJiraIssues(lastSync, _issueClient);

        foreach (var jiraIssue in jiraIssues)
        {
            if (jiraIssue.Fields.Updated <= lastSync)
            {
                continue;
            }

            var actionErrorMessage = ResourceIds.LoadPendenzenErrorFormatMessage;
            var jiraKey = jiraIssue.Key;
            try
            {
                var vxPendenzen = _vinXConnector.LoadVinXPendenzen(jiraKey);

                switch (vxPendenzen.Count)
                {
                    case 0:
                        actionErrorMessage = ResourceIds.CreatePendenzErrorFormatMessage;
                        _vinXConnector.CreatePendenz(jiraIssue);
                        _result.CreatedPendenzenCount++;
                        break;
                    case 1:
                        actionErrorMessage = ResourceIds.UpdatePendenzErrorFormatMessage;
                        _vinXConnector.UpdatePendenz(vxPendenzen.First(), jiraIssue);
                        _result.UpdatedPendenzenCount++;
                        break;
                    default:
                        _messageLogger.AddMessage(MessageTypeEnum.Warning,
                            ResourceIds.MultiplePendenzenErrorFormatMessage, jiraKey);
                        _result.Result = JobResultEnum.PartialSuccess;
                        break;
                }

                actionErrorMessage = ResourceIds.SetLastSyncErrorFormatMessage;
                _vinXConnector.SetLastSync(jiraIssue);
            }
            catch (Exception ex)
            {
                _messageLogger.AddMessage(MessageTypeEnum.Error, actionErrorMessage, jiraKey,
                    LocalizableException.GetExceptionMessage(ex));
                _messageLogger.AddMessage(ex);
                _result.ErrorCount++;
                _result.Result = JobResultEnum.PartialSuccess;
            }
        }

        if (_result.Result == JobResultEnum.Success)
        {
            _result.Result = _vinXConnector.GetMapperResult();
        }

        _vinXConnector.Dispose();

        return _result;
    }
}