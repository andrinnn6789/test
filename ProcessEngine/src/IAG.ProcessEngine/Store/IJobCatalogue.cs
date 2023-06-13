using System.Collections.Generic;

using IAG.ProcessEngine.Store.Model;

namespace IAG.ProcessEngine.Store;

public interface IJobCatalogue
{
    List<IJobMetadata> Catalogue { get; }

    void Reload();
}