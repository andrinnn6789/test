using System.Collections.Generic;

using IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.VinX;

public interface IVersionSyncVinXConnector
{
    void CreateConnection(string connectionString);

    IEnumerable<Version> GetVersions();

    IEnumerable<PendenzSyncProjectSettings> GetProjectSettings();

    void SetIsSyncedToJiraToTrue(Version version);

    void Dispose();
}