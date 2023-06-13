using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.Globalisation.Localizer;

public interface IStringLocalizerFactoryReloadable : IStringLocalizerFactory
{
    void Reload();
}