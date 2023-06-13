using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.ControlCenter.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        AddTemplate(ResourceIds.GetFilesError, "en", "Failed to get files from path '{0}'");
        AddTemplate(ResourceIds.ReadFileContentError, "en", "Failed to read content of file '{0}'");
        AddTemplate(ResourceIds.MainFileNotFoundError, "en", "Main release file not found");
        AddTemplate(ResourceIds.MainFileNoVersionError, "en", "Main release file has no version");
    }
}