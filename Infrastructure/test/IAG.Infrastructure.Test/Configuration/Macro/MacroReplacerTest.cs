using System.Collections.Generic;

using IAG.Infrastructure.Configuration.Macro;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Configuration.Macro;

public class MacroReplacerTest
{
    private readonly MacroReplacer _macroReplacer;
    private readonly Dictionary<string, string> _placeholderDictionary;

    public MacroReplacerTest()
    {
        _placeholderDictionary = new Dictionary<string, string>();

        var macroValueSource = new Mock<IMacroValueSource>();
        macroValueSource.Setup(m => m.GetValue(It.IsAny<string>())).Returns<string>(s => _placeholderDictionary.ContainsKey(s) ? _placeholderDictionary[s] : null);

        _macroReplacer = new MacroReplacer(macroValueSource.Object);
    }

    [Fact]
    public void SimpleStringReplaceTest()
    {
        _placeholderDictionary.Add("Number", "42");
        var testBegin = "$$Number$ at start replacement";
        var testMiddle = "Replace $$Number$ in the middle";
        var testEnd = "The best at the end: $$Number$";
        var testMultiple = "$$Number$ is $$Number$. And $$Number$*1 is still $$Number$";

        var resultBegin = _macroReplacer.ReplaceMacros(testBegin);
        var resultMiddle = _macroReplacer.ReplaceMacros(testMiddle);
        var resultEnd = _macroReplacer.ReplaceMacros(testEnd);
        var resultMultiple = _macroReplacer.ReplaceMacros(testMultiple);

        Assert.Equal("42 at start replacement", resultBegin);
        Assert.Equal("Replace 42 in the middle", resultMiddle);
        Assert.Equal("The best at the end: 42", resultEnd);
        Assert.Equal("42 is 42. And 42*1 is still 42", resultMultiple);
    }

    [Fact]
    public void SimpleJsonReplaceTest()
    {
        _placeholderDictionary.Add("Config", "{ \"Number\": 42, \"Foo\": \"Bar\" }");
        var testJson = "{ \"Config\": \"$Config\" }";

        var resultJson = _macroReplacer.ReplaceMacros(testJson);

        Assert.Equal("{ \"Config\": { \"Number\": 42, \"Foo\": \"Bar\" } }", resultJson);
    }

    [Fact]
    public void NestedReplaceTest()
    {
        _placeholderDictionary.Add("Number", "42");
        _placeholderDictionary.Add("TextWithPlaceHolder", "number $$Number$ is");
        var test = "The $$TextWithPlaceHolder$ in the middle";

        var result = _macroReplacer.ReplaceMacros(test);

        Assert.Equal("The number 42 is in the middle", result);
    }

    [Fact]
    public void NotExistingReplaceTest()
    {
        var testBegin = "$$Number$ at start replacement";
        var testMiddle = "Replace $$Number$ in the middle";
        var testEnd = "The best at the end: $$Number$";
        var testMultiple = "$$Number$ is $$Number$. And $$Number$*1 is still $$Number$";
        var testJson = "{ \"Config\": \"$Config\" }";

        var resultBegin = _macroReplacer.ReplaceMacros(testBegin);
        var resultMiddle = _macroReplacer.ReplaceMacros(testMiddle);
        var resultEnd = _macroReplacer.ReplaceMacros(testEnd);
        var resultMultiple = _macroReplacer.ReplaceMacros(testMultiple);
        var resultJson = _macroReplacer.ReplaceMacros(testJson);

        Assert.Equal(testBegin, resultBegin);
        Assert.Equal(testMiddle, resultMiddle);
        Assert.Equal(testEnd, resultEnd);
        Assert.Equal(testMultiple, resultMultiple);
        Assert.Equal(testJson, resultJson);
    }

    [Fact]
    public void InvalidPlaceHolderTest()
    {
        var testNoEndString = "Replace $$Number in the middle";
        var testWrongStartString = "Replace $Number$ in the middle";
        var testNoEndJson = "{ \"Config\": \"$Config }";
        var testWrongStartJson = "{ \"Config\": $Config\" }";

        var resultNoEndString = _macroReplacer.ReplaceMacros(testNoEndString);
        var resultWrongStartString = _macroReplacer.ReplaceMacros(testWrongStartString);
        var resultNoEndJson = _macroReplacer.ReplaceMacros(testNoEndJson);
        var resultWrongStartJson = _macroReplacer.ReplaceMacros(testWrongStartJson);

        Assert.Equal(testNoEndString, resultNoEndString);
        Assert.Equal(testWrongStartString, resultWrongStartString);
        Assert.Equal(testNoEndJson, resultNoEndJson);
        Assert.Equal(testWrongStartJson, resultWrongStartJson);
    }
}