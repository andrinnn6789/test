using System;

namespace IAG.Infrastructure.IdentityServer.Authorization.Model;

[Flags]
public enum PermissionKind
{
    None = 0,
    Create = 1,
    Read = 2,
    Update = 4,
    Delete = 8,
    Execute = 16,
    Crud = Create | Read | Update | Delete,
    All = Create | Read | Update | Delete | Execute,
    SuperUser = 32
}