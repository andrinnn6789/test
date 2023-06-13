namespace IAG.Infrastructure.Globalisation.Localizer;

public interface ILocalizableObject
{
    string ResourceId { get; }

    object[] Params { get; }
}