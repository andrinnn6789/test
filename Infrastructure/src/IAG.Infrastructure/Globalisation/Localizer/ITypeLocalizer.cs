namespace IAG.Infrastructure.Globalisation.Localizer;

public interface ITypeLocalizer
{
    public string Localize(string prefix, object obj);
}