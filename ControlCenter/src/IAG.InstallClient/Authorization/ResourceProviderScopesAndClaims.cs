using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.InstallClient.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderScopesAndClaims : ResourceProvider
{
    public ResourceProviderScopesAndClaims()
    {
        // Scopes
        AddTemplate(Scopes.InstallerScope, "en", "Scope 'Install' for I-AG Install-Client");
    }
}