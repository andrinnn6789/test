using System;
using System.Collections.Generic;

using IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.VinX;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.VinX;

public interface IWorklogSyncVinXConnector
{
    void CreateConnection(string connectionString);

    void Dispose();

    IEnumerable<Zeit> GetWorkingHours(DateTime lastSync);
        
    IEnumerable<Mitarbeiter> GetEmployees();
        
    IEnumerable<Pendenz> GetPendenzen();
        
    void UpdateWorkingHour(Zeit workingHour);
}