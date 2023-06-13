using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Test.Globalisation.ResourceProvider;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class TestResources : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public TestResources()
    {
        const string resPrefix = "test.";
        AddTemplate(resPrefix + "1", "de", "{0} Zeilen aktualisiert");
        AddTemplate(resPrefix + "2", "de", "{0} Zeilen eingefügt");
        AddTemplate(resPrefix + "3", "en", "{0} rows deleted");
    }
}