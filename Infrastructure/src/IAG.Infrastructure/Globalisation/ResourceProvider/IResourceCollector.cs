using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;

namespace IAG.Infrastructure.Globalisation.ResourceProvider;

public interface IResourceCollector
{
    ResourceContext Context { get; }
    void CollectAndUpdate();
    void SyncTemplatesToResources(string prefix, IList<IResourceTemplate> templates);
}