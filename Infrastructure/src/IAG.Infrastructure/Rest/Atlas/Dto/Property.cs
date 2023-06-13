using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Rest.Atlas.Dto;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Property
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool? Primary { get; set; }

    public bool? Required { get; set; }

    public AtlasType Type { get; set; }

    public string Size { get; set; }

    public List<Link> Links { get; set; }

    public List<KeyValuePair<int, string>> Lookup { get; set; }
}