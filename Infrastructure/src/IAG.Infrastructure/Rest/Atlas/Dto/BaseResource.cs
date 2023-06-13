using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Rest.Atlas.Dto;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class BaseResource
{
    public string Type { get; set; }

    public string Code { get; set; }

    public string Message { get; set; }
}