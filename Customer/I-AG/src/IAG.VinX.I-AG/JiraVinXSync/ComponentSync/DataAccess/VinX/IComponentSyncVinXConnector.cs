using IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.VinX;
using System.Collections.Generic;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.VinX;

public interface IComponentSyncVinXConnector
{
    void CreateConnection(string connectionString);

    public IEnumerable<Component> GetComponents();

    public void SetIsSyncedToJiraToTrue(Component component);

    void Dispose();
}