using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.DataLayer.Model.System;

[ExcludeFromCodeCoverage]
public class Division : BaseEntity, ITenantUniqueNamedEntity
{
    [UsedImplicitly]
    public Tenant Tenant { get; set; }

    [UsedImplicitly]
    public Guid? TenantId { get; set; }

    [MaxLength(32)]
    [UsedImplicitly]
    public string Name { get; set; }
}