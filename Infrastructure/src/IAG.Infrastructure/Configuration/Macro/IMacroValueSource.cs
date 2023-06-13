namespace IAG.Infrastructure.Configuration.Macro;

public interface IMacroValueSource
{
    string GetValue(string placeholder);
}