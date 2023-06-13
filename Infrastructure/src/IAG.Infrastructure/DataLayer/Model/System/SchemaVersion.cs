using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.DataLayer.Model.System;

[ExcludeFromCodeCoverage]
public class SchemaVersion : BaseEntity
{
    [MaxLength(64)]
    [UsedImplicitly]
    public string Module { get; set; }

    [MaxLength(16)]
    [UsedImplicitly]
    public string Version { get; set; }
}