using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Rest.Atlas.Dto;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class AtlasResponse<T>
    where T : new()
{
    public List<Resource<T>> Resource { get; set; }
}