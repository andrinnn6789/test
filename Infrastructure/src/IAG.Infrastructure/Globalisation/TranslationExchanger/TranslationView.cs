using System;

namespace IAG.Infrastructure.Globalisation.TranslationExchanger;

public class TranslationView
{
    public Guid Id { get; set; }

    public string ResourceName { get; set; }

    public string CultureName { get; set; }

    public string Translation { get; set; }
}