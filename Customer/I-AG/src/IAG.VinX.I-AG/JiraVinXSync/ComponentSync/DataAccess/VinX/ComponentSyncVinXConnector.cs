using IAG.Common.DataLayerSybase;
using System;
using System.Collections.Generic;

using IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.VinX;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.VinX;

public class ComponentSyncVinXConnector : IComponentSyncVinXConnector, IDisposable
{
    private readonly ISybaseConnectionFactory _connectionFactory;
    private ISybaseConnection _connection;

    public ComponentSyncVinXConnector(ISybaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void CreateConnection(string connectionString)
    {
        _connection = _connectionFactory.CreateConnection(connectionString);
    }

    public IEnumerable<Component> GetComponents()
    {
        var components = _connection.GetQueryable<Component>();
        return components;
    }

    public void SetIsSyncedToJiraToTrue(Component component)
    {
        component.IsSyncedInJira = true;
        _connection.Update(component);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}