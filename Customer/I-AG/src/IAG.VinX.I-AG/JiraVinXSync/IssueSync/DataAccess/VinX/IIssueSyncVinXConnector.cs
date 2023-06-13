using System;
using System.Collections.Generic;

using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

using Pendenz = IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX.Pendenz;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess.VinX;

public interface IIssueSyncVinXConnector
{
    void CreateConnection(string connectionString);

    void Dispose();

    public DateTime GetLastSync();

    public JobResultEnum GetMapperResult();

    void InitPendenzSyncSettings();

    void InitPendenzSyncDetailSettings();

    void InitMapper(IMessageLogger logger);

    List<Issue> LoadJiraIssues(DateTime lastSync, JiraIssueClient issueClient);

    List<Pendenz> LoadVinXPendenzen(string jiraKey);

    void CreatePendenz(Issue issue);

    void UpdatePendenz(Pendenz pendenz, Issue issue);

    void SetLastSync(Issue issue);
}