namespace IAG.Infrastructure.Globalisation.Model;

public class ResourceTemplate : IResourceTemplate
{
    public ResourceTemplate(string name, string language, string translation)
    {
        Name = name;
        Language = language;
        Translation = translation;
    }

    public string Name { get; }

    public string Language { get; }

    public string Translation { get; }
}