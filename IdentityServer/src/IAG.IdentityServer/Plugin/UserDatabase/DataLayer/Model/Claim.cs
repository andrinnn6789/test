using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Claim : BaseEntityWithTenant
{
    public Guid ScopeId { get; set; }
    public Scope Scope { get; set; }
    public string Name { get; set; }
    public PermissionKind AvailablePermissions { get; set; }
}