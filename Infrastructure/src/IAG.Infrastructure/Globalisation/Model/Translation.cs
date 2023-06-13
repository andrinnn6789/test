using System;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Globalisation.Model;

[UsedImplicitly]
public class Translation : BaseEntity
{
    [UsedImplicitly]
    public Guid ResourceId { get; set; }

    [UsedImplicitly]
    public Guid CultureId { get; set; }

    [UsedImplicitly]
    public Culture Culture { get; set; }

    [UsedImplicitly]
    public Resource Resource { get; set; }

    [UsedImplicitly]
    public string Value { get; set; }
}