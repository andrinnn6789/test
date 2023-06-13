using System;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IdentityServer.Authorization.Model;

[UsedImplicitly]
public class ClaimDefinition
{
    public Guid? TenantId { get; set; }
    public string ScopeName { get; set; }
    public string ClaimName { get; set; }
    public PermissionKind AvailablePermissions { get; set; }
}