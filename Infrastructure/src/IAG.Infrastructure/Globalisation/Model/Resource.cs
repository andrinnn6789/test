using System.Collections.Generic;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Globalisation.Model;

public class Resource : BaseEntity, IUniqueNamedEntity
{
    [UsedImplicitly]
    public string Name { get; set; }

    [UsedImplicitly]
    public IList<Translation> Translations { get; set; }

    public Resource()
    {
        Translations = new List<Translation>();
    }
}