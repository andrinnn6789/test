using JetBrains.Annotations;

namespace IAG.Infrastructure.Globalisation.Model;

public class TranslationFlat
{
    [UsedImplicitly]
    public string Culture { get; set; }

    [UsedImplicitly]
    public string Resource { get; set; }

    [UsedImplicitly]
    public string Value { get; set; }
}