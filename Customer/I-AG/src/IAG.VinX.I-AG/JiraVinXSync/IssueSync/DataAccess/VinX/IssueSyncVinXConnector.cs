using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX;
using IAG.VinX.IAG.Resource;

using Pendenz = IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX.Pendenz;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess.VinX;

public class IssueSyncVinXConnector : IIssueSyncVinXConnector, IDisposable
{
    private readonly ISybaseConnectionFactory _connectionFactory;
    private ISybaseConnection _connection;
    private IssueToPendenzMapper _mapper;

    private PendenzSyncSettings _pendenzSyncSettings;
    private List<PendenzSyncDetailSettings> _vxPendenzSyncDetailSettings;

    public IssueSyncVinXConnector(ISybaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void CreateConnection(string connectionString)
    {
        _connection = _connectionFactory.CreateConnection(connectionString);
    }

    public void InitPendenzSyncSettings()
    {
        try
        {
            _pendenzSyncSettings = _connection.GetQueryable<PendenzSyncSettings>().ToList().FirstOrDefault();
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.LoadPendenzSettingsErrorFormatMessage, ex);
        }

        if (_pendenzSyncSettings == null)
        {
            throw new LocalizableException(ResourceIds.LoadPendenzSettingsErrorFormatMessage, "keine Pendenz-Settings gefunden");
        }
    }

    public void InitPendenzSyncDetailSettings()
    {
        try
        {
            _vxPendenzSyncDetailSettings = _connection.GetQueryable<PendenzSyncDetailSettings>().ToList();
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.LoadPendenzSettingsErrorFormatMessage, ex);
        }
    }

    public void InitMapper(IMessageLogger logger)
    {
        try
        {
            _mapper = new IssueToPendenzMapper(logger, _connection, _pendenzSyncSettings, _vxPendenzSyncDetailSettings);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.InitMapperErrorFormatMessage, ex);
        }
    }

    public List<Issue> LoadJiraIssues(DateTime lastSync, JiraIssueClient issueClient)
    {
        try
        {
            var jiraIssues = issueClient.GetIssues(lastSync);
            return jiraIssues.Result;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.LoadJiraIssuesErrorFormatMessage, ex);
        }
    }

    public List<Pendenz> LoadVinXPendenzen(string jiraKey)
    {
        return _connection.GetQueryable<Pendenz>().Where(p => p.JiraKey == jiraKey).ToList();
    }

    public void CreatePendenz(Issue issue)
    {
        var vinxPendenz = _mapper.NewDestination(issue);

        _connection.ExecuteInTransaction(() =>
        {
            _connection.Insert(vinxPendenz);
            using var cmd = _connection.CreateCommand(@"
                    UPDATE Pendenz 
                    SET Pendenz_PendenzNummer = (SELECT MAX(Pendenz_PendenzNummer) + 1 FROM Pendenz) 
                    WHERE Pendenz_ID  = ?",
                vinxPendenz.ID);
            cmd.ExecuteNonQuery();
        });
    }

    public void UpdatePendenz(Pendenz pendenz, Issue issue)
    {
        var updatedVxPendenz = _mapper.UpdateDestination(pendenz, issue);
        _connection.Update(updatedVxPendenz);
    }

    public void SetLastSync(Issue issue)
    {
        var lastSync = issue.Fields.Updated;
        _pendenzSyncSettings.LastSync = lastSync.ToString("s");

        _connection.Update(_pendenzSyncSettings);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    public DateTime GetLastSync() => _pendenzSyncSettings.LastSync != null ? DateTime.Parse(_pendenzSyncSettings.LastSync) : DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

    public JobResultEnum GetMapperResult() => _mapper.MapperResult;
}