using System.Collections.Generic;

using IAG.Infrastructure.Formatter;

using Xunit;

namespace IAG.Infrastructure.Test.Formatter;

public class WindowsToUnicodeConverterTest
{
    private readonly Dictionary<int, char> _windowsToUnicode = new()
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

    [Fact]
    public void ConvertEmptyString()
    {
        var expected = string.Empty;
        var converted = WindowsToUnicodeConverter.Convert(expected);

        Assert.Equal(expected, converted);
    }

    [Fact]
    public void ConvertString()
    {
        var testString = "Teststring: ";
        var expected = "Teststring: ";
        foreach (var (key, value) in _windowsToUnicode)
        {
            testString += (char) key;
            expected += value;
        }

        var converted = WindowsToUnicodeConverter.Convert(testString);

        Assert.Equal(expected, converted);
    }
}