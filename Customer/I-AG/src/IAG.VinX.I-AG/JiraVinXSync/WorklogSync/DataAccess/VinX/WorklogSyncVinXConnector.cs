using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.VinX;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.VinX;

public class WorklogSyncVinXConnector : IWorklogSyncVinXConnector, IDisposable
{
    private readonly ISybaseConnectionFactory _connectionFactory;
    private ISybaseConnection _connection;
        
    public WorklogSyncVinXConnector(ISybaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void CreateConnection(string connectionString)
    {
        _connection = _connectionFactory.CreateConnection(connectionString);
    }

    public IEnumerable<Zeit> GetWorkingHours(DateTime lastSync)
    {
        var workingHours = _connection.GetQueryable<Zeit>().Where(p => p.Timestamp >= lastSync && p.PendenzID != null);
        return workingHours;
    }
        
    public IEnumerable<Mitarbeiter> GetEmployees()
    {
        var employees = _connection.GetQueryable<Mitarbeiter>();
        return employees;
    }
        
    public IEnumerable<Pendenz> GetPendenzen()
    {
        var issues = _connection.GetQueryable<Pendenz>();
        return issues;
    }

    public void UpdateWorkingHour(Zeit workingHour)
    {
        _connection.Update(workingHour);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}