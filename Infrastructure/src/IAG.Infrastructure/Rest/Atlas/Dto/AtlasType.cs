using JetBrains.Annotations;

namespace IAG.Infrastructure.Rest.Atlas.Dto;

[UsedImplicitly]
public enum AtlasType
{
    Number,
    String,
    Boolean,
    DateTime,
    Base64
}