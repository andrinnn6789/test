using IAG.Common.DataLayerSybase;
using System;
using System.Collections.Generic;

using IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX;

using Version = IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX.Version;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.VinX;

public class VersionSyncVinXConnector : IVersionSyncVinXConnector, IDisposable
{
    private readonly ISybaseConnectionFactory _connectionFactory;
    private ISybaseConnection _connection;

    public VersionSyncVinXConnector(ISybaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void CreateConnection(string connectionString)
    {
        _connection = _connectionFactory.CreateConnection(connectionString);
    }

    public IEnumerable<Version> GetVersions()
    {
        var versions = _connection.GetQueryable<Version>();
        return versions;
    }
    public IEnumerable<PendenzSyncProjectSettings> GetProjectSettings()
    {
        var settings = _connection.GetQueryable<PendenzSyncProjectSettings>();
        return settings;
    }

    public void SetIsSyncedToJiraToTrue(Version version)
    {
        version.IsSyncedInJira = true;
        _connection.Update(version);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}