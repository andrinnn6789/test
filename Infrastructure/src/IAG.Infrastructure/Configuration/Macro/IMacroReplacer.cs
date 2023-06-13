namespace IAG.Infrastructure.Configuration.Macro;

public interface IMacroReplacer
{
    string ReplaceMacros(string config);
}