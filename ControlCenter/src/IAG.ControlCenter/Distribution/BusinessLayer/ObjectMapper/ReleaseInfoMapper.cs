using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;

public class ReleaseInfoMapper : ObjectMapper<Release, ReleaseInfo>
{
    protected override ReleaseInfo MapToDestination(Release source, ReleaseInfo destination)
    {
        destination.Id = source.Id;
        destination.ProductId = source.ProductId;
        destination.ReleaseVersion = source.ReleaseVersion;
        destination.Platform = source.Platform;
        destination.ReleaseDate = source.ReleaseDate;
        destination.Description = source.Description;
        destination.ReleasePath = source.ReleasePath;
        destination.ArtifactPath = source.ArtifactPath;

        return destination;
    }
}