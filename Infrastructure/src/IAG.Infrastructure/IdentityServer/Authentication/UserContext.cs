using System;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace IAG.Infrastructure.IdentityServer.Authentication;

public class UserContext : IUserContext
{
    public static readonly string AnonymousUserName = "Anonymous";

    public string UserName { get; private set; }
    public Guid? TenantId { get; private set; }

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        UserName = httpContextAccessor?.HttpContext?.User.Identity?.Name ?? AnonymousUserName;
        var tenantAsString = httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Tenant)?.Value;
        if (!string.IsNullOrEmpty(tenantAsString) && Guid.TryParse(tenantAsString, out var tenant))
        {
            TenantId = tenant;
        }
    }

    public void SetExplicitUser(string userName, Guid? tenant)
    {
        UserName = userName;
        TenantId = tenant;
    }
}