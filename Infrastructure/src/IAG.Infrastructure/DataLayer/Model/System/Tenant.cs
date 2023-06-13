using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.DataLayer.Model.System;

[ExcludeFromCodeCoverage]
public class Tenant : BaseEntity
{
    [UsedImplicitly]
    public Tenant ParentTenant { get; set; }

    [UsedImplicitly]
    public Guid? ParentTenantId { get; set; }

    [MaxLength(32)]
    [UsedImplicitly]
    public string Name { get; set; }

    [UsedImplicitly]
    public Guid? TenantContactId { get; set; }      // Warning: Don't name it to "ContactId"!
}