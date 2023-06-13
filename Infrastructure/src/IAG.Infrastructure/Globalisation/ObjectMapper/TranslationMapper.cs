using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.Infrastructure.Globalisation.ObjectMapper;

public class TranslationMapper : ObjectMapper<Translation, Translation>
{
    private readonly ResourceContext _context;
    private readonly TranslationSync _translationSync;

    public TranslationMapper(ResourceContext context, TranslationSync translationSync)
    {
        _context = context;
        _translationSync = translationSync;
    }

    protected override Translation MapToDestination(Translation source, Translation destination)
    {
        destination.Id = source.Id;
        destination.Value = source.Value;
        var cult = _translationSync.Cultures.First(s => s.Id.Equals(source.CultureId));
        destination.CultureId = _context.Cultures.Local.First(c => c.Name.Equals(cult.Name)).Id;
        var res = _translationSync.Resources.First(r => r.Id.Equals(source.ResourceId));
        destination.ResourceId = _context.Resources.Local.First(c => c.Name.Equals(res.Name)).Id;
        return destination;
    }
}