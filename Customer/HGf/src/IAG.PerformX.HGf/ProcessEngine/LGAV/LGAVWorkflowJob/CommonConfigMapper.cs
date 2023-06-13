using IAG.Infrastructure.ObjectMapper;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

public class CommonConfigMapper<TDest> : ObjectMapper<ICommonConfig, TDest>
    where TDest : class, ICommonConfig, new()
{
    protected override TDest MapToDestination(ICommonConfig source, TDest destination)
    {
        destination.AtlasCredentials = source.AtlasCredentials;
        destination.AtlasBasePath = source.AtlasBasePath;
        destination.LgavConfig = source.LgavConfig;
        destination.LgavApiKey = source.LgavApiKey;

        return destination;
    }
}