using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Configuration.Model;

public abstract class ConfigBase : BaseEntity
{
    [ExcludeFromCodeCoverage]
    [UsedImplicitly]
    public string Name { get; set; }

    [UsedImplicitly]
    public string Data { get; set; }
}