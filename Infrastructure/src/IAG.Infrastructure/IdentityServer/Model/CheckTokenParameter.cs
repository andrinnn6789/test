using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IdentityServer.Model;

[ExcludeFromCodeCoverage]
public class CheckTokenParameter
{
    [UsedImplicitly]
    public string Token { get; set; }
}