using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.ObjectMapper;

namespace IAG.Infrastructure.Globalisation.Admin;

public class ResourceAdmin
{
    private readonly ResourceContext _context;

    public ResourceAdmin(ResourceContext context)
    {
        _context = context;
    }

    public void SyncSystems(TranslationSync translationSyncs)
    {
        _context.SyncLocalEntitiesByName(translationSyncs.Cultures, new CultureMapper());
        _context.SyncLocalEntitiesByName(translationSyncs.Resources, new ResourceMapper());
        _context.SyncLocalEntities(translationSyncs.Translations, new TranslationMapper(_context, translationSyncs));
        _context.SaveChanges();
    }

    public List<TranslationFlat> GetFlat(string culture)
    {
        var translations = _context.Translations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(culture))
            translations = translations.Where(t => t.Culture.Name.Equals(culture));
        return translations.Select(t => new TranslationFlat
            {
                Culture = t.Culture.Name,
                Resource = t.Resource.Name,
                Value = t.Value
            }
        ).ToList();
    }
}