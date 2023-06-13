using System.Collections.Generic;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Globalisation.Model;

public class Culture : BaseEntity, IUniqueNamedEntity
{
    public string Name { get; set; }

    [UsedImplicitly]
    public IList<Translation> Translations { get; set; }

    public Culture()
    {
        Translations = new List<Translation>();
    }
}