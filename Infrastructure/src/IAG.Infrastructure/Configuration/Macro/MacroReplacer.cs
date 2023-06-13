using System.Text;

namespace IAG.Infrastructure.Configuration.Macro;

public class MacroReplacer : IMacroReplacer
{
    private const char MainPlaceholderStartChar = '$';
    private const char ObjectPlaceholderDelimiter = '"';

    private readonly IMacroValueSource _valueSource;

    public MacroReplacer(IMacroValueSource valueSource)
    {
        _valueSource = valueSource;
    }

    public string ReplaceMacros(string config)
    {
        if (string.IsNullOrEmpty(config))
            return config;

        int placeholderStartPos;
        int placeholderEndPos = 0;
        int lastReplacementEndPos = 0;
        var newConfig = new StringBuilder();
        while ((placeholderStartPos = config.IndexOf(MainPlaceholderStartChar, placeholderEndPos)) >= 0)
        {
            if (placeholderStartPos + 1 < config.Length && config[placeholderStartPos + 1] == MainPlaceholderStartChar)
            {
                // string replacement
                placeholderEndPos = config.IndexOf(MainPlaceholderStartChar, placeholderStartPos + 2);
            }
            else if (placeholderStartPos > 0 && config[placeholderStartPos - 1] == ObjectPlaceholderDelimiter)
            {
                // full JSON replacement
                placeholderEndPos = config.IndexOf(ObjectPlaceholderDelimiter, placeholderStartPos + 1);
                placeholderStartPos--;
            }
            else
            {
                placeholderEndPos = placeholderStartPos + 1;
                continue;
            }

            if (placeholderEndPos > 0)
            {
                var placeholder = config.Substring(placeholderStartPos + 2, placeholderEndPos - placeholderStartPos - 2);
                var replacement = _valueSource.GetValue(placeholder);
                replacement = ReplaceMacros(replacement); 
                if (replacement != null)
                {
                    newConfig.Append(config.Substring(lastReplacementEndPos, placeholderStartPos - lastReplacementEndPos));
                    newConfig.Append(replacement);
                    placeholderEndPos++;
                    lastReplacementEndPos = placeholderEndPos;
                }

            }
            else
            {
                break;
            }
        }

        newConfig.Append(config.Substring(lastReplacementEndPos));

        return newConfig.ToString();
    }
}