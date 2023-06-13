using System;

namespace IAG.Infrastructure.IdentityServer.Authentication;

public interface IUserContext
{
    string UserName { get; }
    Guid? TenantId { get; }

    void SetExplicitUser(string userName, Guid? tenant);
}