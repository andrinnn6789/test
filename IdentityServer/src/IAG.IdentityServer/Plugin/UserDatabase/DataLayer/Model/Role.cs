using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Role : BaseEntityWithTenant, ITenantUniqueNamedEntity
{
    public string Name { get; set; }


    [ForeignKey("RoleId")]
    public List<RoleClaim> RoleClaims { get; set; }
}