using System.Collections.Generic;
using System.Linq;

namespace IAG.Infrastructure.Globalisation.TranslationExchanger;

public class TranslationExcelStructure
{
    public List<string> ExtractCultures(TranslationView[] translationViews)
    {
        return translationViews
            .OrderBy(t => t.CultureName)
            .Select(t => t.CultureName)
            .Distinct()
            .ToList();
    }

    public List<string> ExtractResources(TranslationView[] translationViews)
    {
        return translationViews
            .OrderBy(t => t.ResourceName)
            .Select(t => t.ResourceName)
            .Distinct()
            .ToList();
    }
}