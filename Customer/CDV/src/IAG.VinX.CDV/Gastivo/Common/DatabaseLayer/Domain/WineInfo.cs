using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class WineInfo
{
    public virtual int Id { get; set; }
    [CanBeNull] public virtual string Character { get; set; }
    [CanBeNull] public virtual string CharacterItalian { get; set; }
    [CanBeNull] public virtual string CharacterFrench { get; set; }
    public virtual IList<Recommendation> Recommendations { get; set; }
    public virtual IList<Grape> Grapes { get; set; }
}