using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.Jira;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.VinX;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.Jira;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX;
using IAG.VinX.IAG.Resource;

using Version = IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX.Version;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.BusinessLogic;

public class VersionSyncer : IVersionSyncer
{
    private readonly IVersionSyncVinXConnector _vinXConnector;

    private VersionSyncJiraConnector _jiraConnector;
    private IMessageLogger _messageLogger;

    public VersionSyncer(IVersionSyncVinXConnector vinXConnector)
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
        _jiraConnector = new VersionSyncJiraConnector(httpConfig, requestLogger);
        _messageLogger = messageLogger;
    }

    public IJobResult SyncVersions()
    {
        var result = new JobResult
        {
            Result = JobResultEnum.Success
        };

        try
        {
            var versions = _vinXConnector
                .GetVersions()
                .Where(version => version.SyncToJira && !version.IsSyncedInJira)
                .ToList();

            var settings = _vinXConnector
                .GetProjectSettings()
                .Where(setting => setting.SyncVersionsToJira && setting.KostenstelleID == 158) // Kostenstelle: Produkte
                .ToList();

            UploadVersionsToJira(versions, settings);
            ActivateFlagIsSyncedInVinX(versions);
        }
        catch (Exception ex)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.LoadVersionsToJiraErrorFormatMessage,
                LocalizableException.GetExceptionMessage(ex));
            _messageLogger.AddMessage(ex);

            result.ErrorCount++;
            result.Result = JobResultEnum.Failed;
        }
        finally
        {
            _vinXConnector.Dispose();
            _jiraConnector.Dispose();
        }

        return result;
    }

    private static decimal FormatVersionNumber(decimal versionNumber) =>
        decimal.Round(
            decimal.Parse(versionNumber.ToString(CultureInfo.CurrentCulture)
                .Replace(",",".")
                .Replace(".0", "."), new NumberFormatInfo{NumberDecimalSeparator = "."}),
            1,
            MidpointRounding.ToZero);

    private void UploadVersionsToJira(IEnumerable<Version> versions, IReadOnlyCollection<PendenzSyncProjectSettings> settings)
    {
        foreach (var version in versions.DistinctBy(version => new { V = FormatVersionNumber(version.VersionsNummer), version.KostenartID }))
        {
            var project = settings.First(setting => setting.KostenartID == version.KostenartID).Name;
            var projectId = GetProjectIdByKey(project);
            var jiraVersions = _jiraConnector.GetVersionsFromProject(projectId).Result.ToList();
            var formattedVersionNumber = FormatVersionNumber(version.VersionsNummer).ToString(CultureInfo.CurrentCulture).Replace(",",".");

            if (jiraVersions.Any(jiraVersion =>
                    jiraVersion.Name == formattedVersionNumber))
                continue;

            if (project == null)
                throw new Exception("Project not found in table PendenzSyncProjectSettings!");

            var createVersionRequestModel = new CreateVersionRequestModel
            {
                Description = version.Bezeichnung,
                Name = formattedVersionNumber,
                ReleaseDate = version.ReleaseDatum.ToString("yyyy-MM-dd"),
                Project = project
            };

            _ = _jiraConnector.CreateVersion(createVersionRequestModel).Result;
        }
    }

    private int GetProjectIdByKey(string projectKey) => Convert.ToInt32(_jiraConnector.GetProjects().Result.ToList()
        .First(project => project.Key == projectKey).Id);

    private void ActivateFlagIsSyncedInVinX(List<Version> versions)
    {
        foreach (var version in versions)
        {
            _vinXConnector.SetIsSyncedToJiraToTrue(version);
        }
    }
}