using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.IdentityServer.Model;

[ExcludeFromCodeCoverage]
public class ChangePasswordParameter
{
    public string Realm { get; set; }

    public string OldPassword { get; set; }

    public string NewPassword { get; set; }
}