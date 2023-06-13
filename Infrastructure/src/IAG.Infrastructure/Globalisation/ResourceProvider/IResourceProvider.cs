using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Model;

namespace IAG.Infrastructure.Globalisation.ResourceProvider;

public interface IResourceProvider
{
    IEnumerable<IResourceTemplate> ResourceTemplates { get; }

    void AddTemplate(string name, string language, string translation);
}