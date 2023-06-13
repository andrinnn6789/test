using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class UserClaim : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid ClaimId { get; set; }
    public Claim Claim { get; set; }
    public PermissionKind AllowedPermissions { get; set; }
}