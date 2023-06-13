using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.Greiner.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // jobs
        AddTemplate(ResourceIds.ExtractorJobName, "de", "Datei-Export zu ESL");
    }
}