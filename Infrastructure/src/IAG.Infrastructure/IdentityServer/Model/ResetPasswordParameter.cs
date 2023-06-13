using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.IdentityServer.Model;

[ExcludeFromCodeCoverage]
public class ResetPasswordParameter
{
    public string Realm { get; set; }

    public string User { get; set; }

    public Guid? TenantId { get; set; }

    public string UserLanguage { get; set; }
}