
using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.IdentityServer.Authentication;

[ExcludeFromCodeCoverage]
public class ExplicitUserContext : IUserContext
{
    public string UserName { get; private set; }
    public Guid? TenantId { get; private set; }

    public ExplicitUserContext(string userName, Guid? tenant)
    {
        UserName = userName;
        TenantId = tenant;
    }

    public void SetExplicitUser(string userName, Guid? tenant)
    {
        UserName = userName;
        TenantId = tenant;
    }
}