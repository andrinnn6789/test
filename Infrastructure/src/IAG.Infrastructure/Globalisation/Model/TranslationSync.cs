using System.Collections.Generic;

namespace IAG.Infrastructure.Globalisation.Model;

public class TranslationSync
{
    public List<Culture> Cultures { get; set; } = new();

    public List<Resource> Resources { get; set; } = new();

    public List<Translation> Translations { get; set; } = new();
}