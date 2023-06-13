using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.System;

using JetBrains.Annotations;

namespace IAG.Infrastructure.DataLayer.Model.Base;

[ExcludeFromCodeCoverage]
public abstract class BaseEntityWithTenant : BaseEntity
{
    [UsedImplicitly]
    public Tenant Tenant { get; set; }

    [UsedImplicitly]
    public Guid? TenantId { get; set; }

    [UsedImplicitly]
    public Division Division { get; set; }

    [UsedImplicitly]
    public Guid? DivisionId { get; set; }
}