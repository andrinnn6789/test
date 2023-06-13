using System.Collections.Generic;
using System.Text;

namespace IAG.Infrastructure.Formatter;

public static class WindowsToUnicodeConverter
{
    private static readonly Dictionary<int, char> WindowsToUnicode = new()
    {
        {130, '‚'},
        {131, 'ƒ'},
        {132, '„'},
        {133, '…'},
        {134, '†'},
        {135, '‡'},
        {136, 'ˆ'},
        {137, '‰'},
        {138, 'Š'},
        {139, '‹'},
        {140, 'Œ'},
        {145, '‘'},
        {146, '’'},
        {147, '“'},
        {148, '”'},
        {149, '•'},
        {150, '–'},
        {151, '—'},
        {152, '˜'},
        {153, '™'},
        {154, 'š'},
        {155, '›'},
        {156, 'œ'},
        {159, 'Ÿ'},
        {173, '-'}
    };

    /// <summary>
    /// Convert Windows-1252 chars to unicode
    /// </summary>
    /// <param name="inputText">Windows-1252 coded text</param>
    /// <returns>Converted text in unicode</returns>
    public static string Convert(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            return inputText;
        }

        var stringBuilder = new StringBuilder();
        foreach (var inputChar in inputText)
        {
            var code = (int)inputChar;

            if (code >= 130 && code <= 173 && WindowsToUnicode.TryGetValue(code, out var mappedChar))
            {
                stringBuilder.Append(mappedChar);
                continue;
            }

            stringBuilder.Append(inputChar);
        }

        return stringBuilder.ToString();
    }
}