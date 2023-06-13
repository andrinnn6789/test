using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Rest.Atlas.Dto;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Meta
{
    public string Name { get; set; }

    public string Resource { get; set; }

    public int Cardinality { get; set; }

    public string Description { get; set; }

    public List<Property> Properties { get; set; }
}